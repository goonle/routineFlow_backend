using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutineFlow.Middleware;

namespace RoutineFlow.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public class MeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { userId = User.GetUserId(), email = User.FindFirstValue(ClaimTypes.Email) });
    }
}
