using Microsoft.Extensions.Options;
using RoutineFlow.Common;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.BackgroundServices;

public class GoalRetentionBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly GoalRetentionSettings _settings;
    private readonly ILogger<GoalRetentionBackgroundService> _logger;

    public GoalRetentionBackgroundService(
        IServiceScopeFactory scopeFactory,
        IOptions<GoalRetentionSettings> settings,
        ILogger<GoalRetentionBackgroundService> logger)
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
                using var scope = _scopeFactory.CreateScope();
                var retentionService = scope.ServiceProvider.GetRequiredService<IGoalRetentionService>();
                var result = await retentionService.RunAsync();

                if (result.OneMonthWarnings + result.OneWeekWarnings + result.Purged > 0)
                {
                    _logger.LogInformation(
                        "Goal retention run: {OneMonth} one-month warnings, {OneWeek} one-week warnings, {Purged} purged",
                        result.OneMonthWarnings, result.OneWeekWarnings, result.Purged);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GoalRetentionBackgroundService run failed");
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
}
