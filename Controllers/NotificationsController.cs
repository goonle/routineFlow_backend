using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutineFlow.DTOs.Notifications;
using RoutineFlow.Middleware;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IWeeklySummaryService _weeklySummaryService;
    private readonly IWebHostEnvironment _env;

    public NotificationsController(
        INotificationService notificationService,
        IWeeklySummaryService weeklySummaryService,
        IWebHostEnvironment env)
    {
        _notificationService = notificationService;
        _weeklySummaryService = weeklySummaryService;
        _env = env;
    }

    [HttpGet]
    public async Task<ActionResult<NotificationListResponse>> Get(
        [FromQuery] bool? unreadOnly, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _notificationService.GetAsync(User.GetUserId(), unreadOnly, page, pageSize);
        return Ok(result);
    }

    [HttpPut("{id:guid}/read")]
    public async Task<ActionResult<NotificationResponse>> MarkRead(Guid id)
    {
        var notification = await _notificationService.MarkReadAsync(User.GetUserId(), id);
        return Ok(notification);
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        await _notificationService.MarkAllReadAsync(User.GetUserId());
        return NoContent();
    }

    [HttpPost("trigger-weekly-summary")]
    public async Task<IActionResult> TriggerWeeklySummary()
    {
        if (!_env.IsDevelopment())
        {
            return NotFound();
        }

        var created = await _weeklySummaryService.GenerateForAllUsersAsync();
        return Ok(new { created });
    }
}
