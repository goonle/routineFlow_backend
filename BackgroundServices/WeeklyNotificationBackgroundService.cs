using Microsoft.Extensions.Options;
using RoutineFlow.Common;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.BackgroundServices;

public class WeeklyNotificationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly WeeklyNotificationSettings _settings;
    private readonly ILogger<WeeklyNotificationBackgroundService> _logger;

    public WeeklyNotificationBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<WeeklyNotificationSettings> settings,
        ILogger<WeeklyNotificationBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromMinutes(Math.Max(1, _settings.CheckIntervalMinutes));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunIfDueAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WeeklyNotificationBackgroundService run failed");
            }

            try
            {
                await Task.Delay(interval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
            }
        }
    }

    private async Task RunIfDueAsync()
    {
        var now = DateTime.UtcNow;
        var currentWeekStart = StartOfWeek(DateOnly.FromDateTime(now));
        var offsetDays = ((int)_settings.DayOfWeek + 6) % 7;
        var fireAt = DateTime.SpecifyKind(
            currentWeekStart.AddDays(offsetDays).ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)
            .AddHours(_settings.Hour);

        if (now < fireAt)
        {
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var weeklySummaryService = scope.ServiceProvider.GetRequiredService<IWeeklySummaryService>();
        var created = await weeklySummaryService.GenerateForAllUsersAsync();

        if (created > 0)
        {
            _logger.LogInformation("Generated {Count} weekly summary notifications", created);
        }
    }

    private static DateOnly StartOfWeek(DateOnly date)
    {
        var daysSinceMonday = ((int)date.DayOfWeek + 6) % 7;
        return date.AddDays(-daysSinceMonday);
    }
}
