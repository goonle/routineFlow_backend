using System.Net;
using System.Net.Http.Json;
using RoutineFlow.DTOs.Calendar;
using RoutineFlow.DTOs.Goals;
using RoutineFlow.DTOs.TaskCompletions;
using RoutineFlow.DTOs.Tasks;

namespace RoutineFlow.IntegrationTests;

[Collection("RoutineFlow")]
public class GoldenPathTests
{
    private readonly RoutineFlowWebApplicationFactory _factory;

    public GoldenPathTests(RoutineFlowWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateGoalTaskCompleteTask_MarksGoalAchievedInCalendarToday()
    {
        var (client, _) = await _factory.RegisterNewUserAsync();

        var goalResponse = await client.PostAsJsonAsync("/api/goals", new CreateGoalRequest { Name = "Exercise" });
        var goal = await goalResponse.Content.ReadFromJsonAsync<GoalResponse>();

        var taskResponse = await client.PostAsJsonAsync($"/api/goals/{goal!.Id}/tasks", new CreateTaskRequest
        {
            Name = "Run 5k",
            AchieveAction = "Go for a run"
        });
        Assert.Equal(HttpStatusCode.Created, taskResponse.StatusCode);
        var task = await taskResponse.Content.ReadFromJsonAsync<TaskResponse>();

        var completeResponse = await client.PostAsJsonAsync($"/api/tasks/{task!.Id}/completions", new MarkCompletionRequest());
        Assert.Equal(HttpStatusCode.OK, completeResponse.StatusCode);
        var completion = await completeResponse.Content.ReadFromJsonAsync<TaskCompletionResponse>();
        Assert.Equal(1, completion!.Count);

        var calendarResponse = await client.GetAsync("/api/calendar/weekly");
        Assert.Equal(HttpStatusCode.OK, calendarResponse.StatusCode);
        var days = await calendarResponse.Content.ReadFromJsonAsync<List<CalendarDayResponse>>();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var todayEntry = days!.Single(d => d.Date == today);
        var goalEntry = todayEntry.Goals.Single(g => g.GoalId == goal.Id);
        Assert.True(goalEntry.Achieved);
    }
}
