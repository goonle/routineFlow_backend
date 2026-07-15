using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutineFlow.DTOs.TaskCompletions;
using RoutineFlow.Middleware;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Controllers;

[ApiController]
[Route("api/tasks/{taskId:guid}/completions")]
[Authorize]
public class TaskCompletionsController : ControllerBase
{
    private readonly ITaskCompletionService _completionService;

    public TaskCompletionsController(ITaskCompletionService completionService)
    {
        _completionService = completionService;
    }

    [HttpPost]
    public async Task<ActionResult<TaskCompletionResponse>> MarkDone(Guid taskId, MarkCompletionRequest request)
    {
        var completion = await _completionService.MarkDoneAsync(User.GetUserId(), taskId, request);
        return Ok(completion);
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskCompletionResponse>>> GetHistory(
        Guid taskId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
    {
        var history = await _completionService.GetHistoryAsync(User.GetUserId(), taskId, from, to);
        return Ok(history);
    }

    [HttpDelete("{date}")]
    public async Task<IActionResult> Undo(Guid taskId, DateOnly date)
    {
        await _completionService.UndoAsync(User.GetUserId(), taskId, date);
        return NoContent();
    }
}
