using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutineFlow.DTOs.Tasks;
using RoutineFlow.Middleware;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Controllers;

[ApiController]
[Route("api/goals/{goalId:guid}/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TaskResponse>>> List(Guid goalId)
    {
        var tasks = await _taskService.ListAsync(User.GetUserId(), goalId);
        return Ok(tasks);
    }

    [HttpGet("{taskId:guid}")]
    public async Task<ActionResult<TaskResponse>> Get(Guid goalId, Guid taskId)
    {
        var task = await _taskService.GetAsync(User.GetUserId(), goalId, taskId);
        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(Guid goalId, CreateTaskRequest request)
    {
        var task = await _taskService.CreateAsync(User.GetUserId(), goalId, request);
        return CreatedAtAction(nameof(Get), new { goalId, taskId = task.Id }, task);
    }

    [HttpPut("{taskId:guid}")]
    public async Task<ActionResult<TaskResponse>> Update(Guid goalId, Guid taskId, UpdateTaskRequest request)
    {
        var task = await _taskService.UpdateAsync(User.GetUserId(), goalId, taskId, request);
        return Ok(task);
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> Delete(Guid goalId, Guid taskId)
    {
        await _taskService.DeleteAsync(User.GetUserId(), goalId, taskId);
        return NoContent();
    }
}
