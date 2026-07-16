using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutineFlow.DTOs.Calendar;
using RoutineFlow.Middleware;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Controllers;

[ApiController]
[Route("api/calendar")]
[Authorize]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _calendarService;

    public CalendarController(ICalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    [HttpGet("weekly")]
    public async Task<ActionResult<List<CalendarDayResponse>>> GetWeekly([FromQuery] DateOnly? startDate)
    {
        var days = await _calendarService.GetWeeklyAsync(User.GetUserId(), startDate);
        return Ok(days);
    }

    [HttpGet("monthly")]
    public async Task<ActionResult<List<CalendarDayResponse>>> GetMonthly([FromQuery] int year, [FromQuery] int month)
    {
        var days = await _calendarService.GetMonthlyAsync(User.GetUserId(), year, month);
        return Ok(days);
    }
}
