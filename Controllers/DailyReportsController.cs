using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutineFlow.DTOs.DailyReports;
using RoutineFlow.Middleware;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Controllers;

[ApiController]
[Route("api/daily-reports")]
[Authorize]
public class DailyReportsController : ControllerBase
{
    private readonly IDailyReportService _dailyReportService;

    public DailyReportsController(IDailyReportService dailyReportService)
    {
        _dailyReportService = dailyReportService;
    }

    [HttpGet("{date}")]
    public async Task<ActionResult<DailyReportResponse>> Get(DateOnly date)
    {
        var report = await _dailyReportService.GetAsync(User.GetUserId(), date);
        return Ok(report);
    }

    [HttpPut("{date}")]
    public async Task<ActionResult<DailyReportResponse>> Upsert(DateOnly date, UpsertDailyReportRequest request)
    {
        var report = await _dailyReportService.UpsertAsync(User.GetUserId(), date, request);
        return Ok(report);
    }
}
