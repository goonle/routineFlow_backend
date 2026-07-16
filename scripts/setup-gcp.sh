#!/usr/bin/env bash
# One-time GCP setup for RoutineFlow's Cloud Run deploy. Run this in Cloud Shell
# (console.cloud.google.com) against the GCP project you want to deploy into.
#
# What this creates: an Artifact Registry repo for the Docker image, a runtime
# service account (the identity Cloud Run runs as), two Secret Manager secrets
# (db connection string + JWT signing key), a deploy service account (the
# identity GitHub Actions uses), and Workload Identity Federation so GitHub
# Actions can authenticate as that deploy service account without a long-lived
# JSON key ever being stored as a GitHub secret.
set -euo pipefail

# ---- Edit these if you want something other than the defaults ----
REGION=us-central1
REPO=routineflow
RUNTIME_SA=routineflow-runtime
DEPLOY_SA=github-deployer
GITHUB_REPO=goonle/routineFlow_backend
POOL_ID=github-pool
PROVIDER_ID=github-provider
# --------------------------------------------------------------------

PROJECT_ID=$(gcloud config get-value project)
PROJECT_NUMBER=$(gcloud projects describe "$PROJECT_ID" --format='value(projectNumber)')
echo "Project: $PROJECT_ID ($PROJECT_NUMBER), region: $REGION"

echo "==> Enabling required APIs..."
gcloud services enable run.googleapis.com artifactregistry.googleapis.com \
  secretmanager.googleapis.com iamcredentials.googleapis.com

echo "==> Creating Artifact Registry repo..."
gcloud artifacts repositories create "$REPO" \
  --repository-format=docker --location="$REGION" \
  --description="RoutineFlow API images"

echo "==> Creating runtime service account (identity Cloud Run runs as)..."
gcloud iam service-accounts create "$RUNTIME_SA" \
  --display-name="RoutineFlow Cloud Run runtime"

echo "==> Creating secrets — paste values interactively, then Ctrl-D. Nothing is echoed or logged."
echo "Paste the Neon POOLED connection string in Npgsql keyword=value form"
echo "(Host=...;Port=5432;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true):"
gcloud secrets create db-connection-string --data-file=-

openssl rand -base64 32 | gcloud secrets create jwt-signing-key --data-file=-

echo "==> Granting the runtime service account access to both secrets..."
for SECRET in db-connection-string jwt-signing-key; do
  gcloud secrets add-iam-policy-binding "$SECRET" \
    --member="serviceAccount:${RUNTIME_SA}@${PROJECT_ID}.iam.gserviceaccount.com" \
    --role="roles/secretmanager.secretAccessor"
done

echo "==> Creating deploy service account (identity GitHub Actions uses)..."
gcloud iam service-accounts create "$DEPLOY_SA" \
  --display-name="GitHub Actions deployer"

for ROLE in roles/run.admin roles/artifactregistry.writer roles/iam.serviceAccountUser; do
  gcloud projects add-iam-policy-binding "$PROJECT_ID" \
    --member="serviceAccount:${DEPLOY_SA}@${PROJECT_ID}.iam.gserviceaccount.com" \
    --role="$ROLE"
done

# Needed so the CI migration step can read the connection string before deploying.
gcloud secrets add-iam-policy-binding db-connection-string \
  --member="serviceAccount:${DEPLOY_SA}@${PROJECT_ID}.iam.gserviceaccount.com" \
  --role="roles/secretmanager.secretAccessor"

echo "==> Setting up Workload Identity Federation (no long-lived key ever stored in GitHub)..."
gcloud iam workload-identity-pools create "$POOL_ID" \
  --location="global" --display-name="GitHub Actions pool"

gcloud iam workload-identity-pools providers create-oidc "$PROVIDER_ID" \
  --location="global" --workload-identity-pool="$POOL_ID" \
  --display-name="GitHub provider" \
  --attribute-mapping="google.subject=assertion.sub,attribute.repository=assertion.repository" \
  --attribute-condition="assertion.repository=='${GITHUB_REPO}'" \
  --issuer-uri="https://token.actions.githubusercontent.com"

gcloud iam service-accounts add-iam-policy-binding \
  "${DEPLOY_SA}@${PROJECT_ID}.iam.gserviceaccount.com" \
  --role="roles/iam.workloadIdentityUser" \
  --member="principalSet://iam.googleapis.com/projects/${PROJECT_NUMBER}/locations/global/workloadIdentityPools/${POOL_ID}/attribute.repository/${GITHUB_REPO}"

echo
echo "==================================================================="
echo "Done. Add these as GitHub Actions repo secrets"
echo "(Settings -> Secrets and variables -> Actions -> New repository secret):"
echo "==================================================================="
echo "GCP_PROJECT_ID=$PROJECT_ID"
echo "GCP_REGION=$REGION"
echo "GCP_ARTIFACT_REPO=$REPO"
echo "GCP_RUNTIME_SA_EMAIL=${RUNTIME_SA}@${PROJECT_ID}.iam.gserviceaccount.com"
echo "GCP_DEPLOY_SA_EMAIL=${DEPLOY_SA}@${PROJECT_ID}.iam.gserviceaccount.com"
echo "GCP_WORKLOAD_IDENTITY_PROVIDER=projects/${PROJECT_NUMBER}/locations/global/workloadIdentityPools/${POOL_ID}/providers/${PROVIDER_ID}"
