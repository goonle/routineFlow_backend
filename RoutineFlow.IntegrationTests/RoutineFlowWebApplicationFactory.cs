using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using RoutineFlow.Data;
using Testcontainers.PostgreSql;

namespace RoutineFlow.IntegrationTests;

public class RoutineFlowWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string SigningKey = "integration-test-signing-key-at-least-32-characters-long";

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("routineflow_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IHostedService));
        });
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        // Program.cs reads Jwt:SigningKey into a local variable at the top of
        // WebApplication.CreateBuilder(args), before WebApplicationFactory's
        // ConfigureWebHost/ConfigureAppConfiguration hooks are merged in. Environment
        // variables are one of the earliest configuration sources CreateBuilder wires
        // up, so setting them here (before the host is first built via the Services
        // accessor below) is what actually reaches that early read.
        Environment.SetEnvironmentVariable("ConnectionStrings__Default", _postgres.GetConnectionString());
        Environment.SetEnvironmentVariable("Jwt__SigningKey", SigningKey);

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await base.DisposeAsync();
    }
}
