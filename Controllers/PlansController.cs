using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutineFlow.DTOs.Plans;
using RoutineFlow.Middleware;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Controllers;

[ApiController]
[Route("api/goals/{goalId:guid}/plan")]
[Authorize]
public class PlansController : ControllerBase
{
    private readonly IPlanService _planService;

    public PlansController(IPlanService planService)
    {
        _planService = planService;
    }

    [HttpGet]
    public async Task<ActionResult<PlanResponse>> Get(Guid goalId)
    {
        var plan = await _planService.GetAsync(User.GetUserId(), goalId);
        return Ok(plan);
    }

    [HttpPut]
    public async Task<ActionResult<PlanResponse>> Upsert(Guid goalId, UpsertPlanRequest request)
    {
        var plan = await _planService.UpsertAsync(User.GetUserId(), goalId, request);
        return Ok(plan);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid goalId)
    {
        await _planService.DeleteAsync(User.GetUserId(), goalId);
        return NoContent();
    }
}
