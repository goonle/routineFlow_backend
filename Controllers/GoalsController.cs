using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutineFlow.DTOs.Goals;
using RoutineFlow.Middleware;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Controllers;

[ApiController]
[Route("api/goals")]
[Authorize]
public class GoalsController : ControllerBase
{
    private readonly IGoalService _goalService;

    public GoalsController(IGoalService goalService)
    {
        _goalService = goalService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GoalResponse>>> List()
    {
        var goals = await _goalService.ListAsync(User.GetUserId());
        return Ok(goals);
    }

    [HttpGet("deleted")]
    public async Task<ActionResult<List<GoalResponse>>> ListDeleted()
    {
        var goals = await _goalService.ListDeletedAsync(User.GetUserId());
        return Ok(goals);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GoalResponse>> Get(Guid id)
    {
        var goal = await _goalService.GetAsync(User.GetUserId(), id);
        return Ok(goal);
    }

    [HttpPost]
    public async Task<ActionResult<GoalResponse>> Create(CreateGoalRequest request)
    {
        var goal = await _goalService.CreateAsync(User.GetUserId(), request);
        return CreatedAtAction(nameof(Get), new { id = goal.Id }, goal);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<GoalResponse>> Update(Guid id, UpdateGoalRequest request)
    {
        var goal = await _goalService.UpdateAsync(User.GetUserId(), id, request);
        return Ok(goal);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _goalService.DeleteAsync(User.GetUserId(), id);
        return NoContent();
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<ActionResult<GoalResponse>> Restore(Guid id)
    {
        var goal = await _goalService.RestoreAsync(User.GetUserId(), id);
        return Ok(goal);
    }
}
