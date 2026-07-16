using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using RoutineFlow.DTOs.Auth;

namespace RoutineFlow.IntegrationTests;

public static class TestClientExtensions
{
    public static async Task<(HttpClient Client, AuthResponse Auth)> RegisterNewUserAsync(this WebApplicationFactory<Program> factory)
    {
        var client = factory.CreateClient();
        var email = $"test+{Guid.NewGuid():N}@example.com";

        var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Email = email,
            Password = "Password123!"
        });
        response.EnsureSuccessStatusCode();

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new InvalidOperationException("Register response did not contain an auth payload.");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);

        return (client, auth);
    }
}
