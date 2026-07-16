# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

RoutineFlow backend: an ASP.NET Core Web API (C#, .NET 8) for a habit/routine-tracking app. Users create **Goals**, break them into **Tasks**, attach a **Plan** (Weekly/Monthly/Custom date range), log daily task completions, and record a daily emotion + diary entry. Progress aggregates into weekly/monthly calendar views and a weekly summary notification.

See `README.md` for the full feature list and product intent, and `C:\Users\junhu\.claude\plans\i-wanna-make-a-nifty-iverson.md` for the original approved implementation plan (domain model rationale, milestone breakdown, and explicitly noted assumptions/ambiguity resolutions — consult it before making a design decision that contradicts what's already built).

## Commands

```bash
# Start PostgreSQL (if not already running)
docker start routineflow-pg   # or: docker run -d --name routineflow-pg -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=routineflow -p 5432:5432 postgres:16

# Build
dotnet build

# Run (dev)
dotnet run
# Swagger UI at https://localhost:<port>/swagger — use the Authorize button with a JWT access token

# EF Core migrations (dotnet-ef must be installed: dotnet tool install --global dotnet-ef)
dotnet ef migrations add <Name>
dotnet ef database update

# Secrets (never commit these — see appsettings.json which intentionally omits them)
dotnet user-secrets list
dotnet user-secrets set "ConnectionStrings:Default" "..."
dotnet user-secrets set "Jwt:SigningKey" "..."
```

`RoutineFlow.IntegrationTests` holds `WebApplicationFactory<Program>` + Testcontainers-Postgres integration tests (`dotnet test`) — a fresh disposable Postgres container per test run, migrated via `Database.MigrateAsync()`, with hosted `BackgroundServices` removed from the test host for determinism. There is no linter configured beyond the default Roslyn/.NET build warnings (`dotnet build` treats these as informational, not CI-enforced).

## Architecture

Single-project layered architecture (not Clean/Onion — this was a deliberate choice for a solo MVP project, see the plan doc):

```
Controllers/    → thin, [Authorize]-by-default API endpoints (except /api/auth/*)
Services/       → business logic, one interface+impl per feature area
Repositories/   → EF Core data access, one interface+impl per entity
Models/
  Entities/     → EF Core entity classes (POCOs)
  Enums/        → PlanType, EmotionType (+ EmotionMetadata emoji lookup), NotificationType
DTOs/           → request/response contracts, grouped by feature folder
Data/
  ApplicationDbContext.cs
  Configurations/  → IEntityTypeConfiguration<T> per entity (Fluent API), auto-applied via
                     modelBuilder.ApplyConfigurationsFromAssembly
  Migrations/
Middleware/     → ExceptionHandlingMiddleware (maps exceptions → ProblemDetails-style JSON),
                   ClaimsPrincipalExtensions.GetUserId() (pulls user id out of JWT claims)
Common/         → JwtSettings, custom exceptions (NotFoundException, ConflictException, UnauthorizedException)
BackgroundServices/  → scheduled jobs (empty until milestone 6/7 — see plan doc)
```

Data flow: `Controller` → `Service` (interface-injected) → `Repository` (interface-injected) → `ApplicationDbContext`. Controllers never touch `ApplicationDbContext` or repositories directly.

### Auth

JWT access token (short-lived, ~15 min) + rotating refresh token. Refresh tokens are stored **hashed** (SHA-256) in the `RefreshTokens` table, never in plaintext; each refresh call revokes the old token and issues a new one (`RevokedAt` + `ReplacedByTokenHash`), so reuse of a stale refresh token is detectable. Passwords are hashed with BCrypt (work factor 12).

Every non-auth controller/service must scope queries to the current user via `User.GetUserId()` (the `ClaimsPrincipalExtensions` helper) — there is no separate multi-tenancy middleware, so ownership checks are the service layer's responsibility on every read/write.

### Entity naming

The task entity is named `TaskItem`, not `Task`, to avoid colliding with `System.Threading.Tasks.Task` (this project uses `ImplicitUsings`, so `System.Threading.Tasks` is in scope everywhere).

### Domain model quirks (see the plan doc for full rationale)

- **Goal achievement is computed, not stored**: a Goal counts as "achieved" for a given day if *any* of its Tasks has a `TaskCompletion` row with `Count > 0` on that date. There is intentionally no caching table for this in the MVP.
- **TaskCompletion is an upsert on `(TaskId, Date)`**, not an insert-per-event: marking a task done again the same day increments `Count` on the existing row rather than creating a new row (unique index enforces this).
- **Goal soft-delete + retention**: `DELETE /api/goals/{id}` sets `Goal.DeletedAt` rather than removing the row. A goal is recoverable for 6 months via a restore endpoint. `Goal.OneMonthWarningSentAt` / `OneWeekWarningSentAt` track whether the 1-month/1-week pre-purge warning notifications have already fired, to keep the (not-yet-built) `GoalRetentionBackgroundService` idempotent.
- **DateOnly, not DateTime**, is used for all "calendar day" concepts (`TaskCompletion.Date`, `DailyReport.Date`, `Plan.StartDate`/`EndDate`) to sidestep time-of-day/timezone ambiguity. The client is expected to send explicit date values rather than relying on server-inferred "today".
- `Notification.Content` is a JSON-serialized payload string (not a rigid column-per-field schema), so a future email-sending channel can be added without a domain model change.

## Secrets / configuration

`appsettings.json` intentionally does not contain `ConnectionStrings:Default` or `Jwt:SigningKey` — both are supplied via `dotnet user-secrets` in development. Don't add real secrets to `appsettings*.json`.

## Working style notes

- This project is being built milestone-by-milestone per the approved plan doc; when adding a new feature area, check whether it's already scoped there before redesigning it.
- The user is using this repo as a public portfolio piece for job applications (see `README.md`'s "Why this project exists") — keep the README's framing in mind if asked to update it, and don't introduce anything embarrassing/broken into `master`.
