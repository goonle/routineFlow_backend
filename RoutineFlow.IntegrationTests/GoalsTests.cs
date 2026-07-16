using System.Net;
using System.Net.Http.Json;
using RoutineFlow.DTOs.Goals;

namespace RoutineFlow.IntegrationTests;

[Collection("RoutineFlow")]
public class GoalsTests
{
    private readonly RoutineFlowWebApplicationFactory _factory;

    public GoalsTests(RoutineFlowWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateGoal_ThenList_ReturnsGoal()
    {
        var (client, _) = await _factory.RegisterNewUserAsync();

        var createResponse = await client.PostAsJsonAsync("/api/goals", new CreateGoalRequest
        {
            Name = "Read more books",
            Description = "30 minutes a day"
        });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<GoalResponse>();
        Assert.NotNull(created);

        var listResponse = await client.GetAsync("/api/goals");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var goals = await listResponse.Content.ReadFromJsonAsync<List<GoalResponse>>();
        Assert.Contains(goals!, g => g.Id == created!.Id && g.Name == "Read more books");
    }

    [Fact]
    public async Task CreateGoal_WithEmptyName_ReturnsValidationProblem()
    {
        var (client, _) = await _factory.RegisterNewUserAsync();

        var response = await client.PostAsJsonAsync("/api/goals", new CreateGoalRequest { Name = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task SecondUser_CannotSeeOrAccessFirstUsersGoal()
    {
        var (client1, _) = await _factory.RegisterNewUserAsync();
        var (client2, _) = await _factory.RegisterNewUserAsync();

        var createResponse = await client1.PostAsJsonAsync("/api/goals", new CreateGoalRequest { Name = "Private Goal" });
        var goal = await createResponse.Content.ReadFromJsonAsync<GoalResponse>();

        var user2List = await client2.GetAsync("/api/goals");
        var user2Goals = await user2List.Content.ReadFromJsonAsync<List<GoalResponse>>();
        Assert.DoesNotContain(user2Goals!, g => g.Id == goal!.Id);

        var user2Get = await client2.GetAsync($"/api/goals/{goal!.Id}");
        Assert.Equal(HttpStatusCode.NotFound, user2Get.StatusCode);
    }

    [Fact]
    public async Task DeleteGoal_SoftDeletes_ThenRestoreBringsItBack()
    {
        var (client, _) = await _factory.RegisterNewUserAsync();

        var createResponse = await client.PostAsJsonAsync("/api/goals", new CreateGoalRequest { Name = "Temp Goal" });
        var goal = await createResponse.Content.ReadFromJsonAsync<GoalResponse>();

        var deleteResponse = await client.DeleteAsync($"/api/goals/{goal!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getAfterDelete = await client.GetAsync($"/api/goals/{goal.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfterDelete.StatusCode);

        var deletedList = await client.GetAsync("/api/goals/deleted");
        var deletedGoals = await deletedList.Content.ReadFromJsonAsync<List<GoalResponse>>();
        Assert.Contains(deletedGoals!, g => g.Id == goal.Id);

        var restoreResponse = await client.PostAsync($"/api/goals/{goal.Id}/restore", null);
        Assert.Equal(HttpStatusCode.OK, restoreResponse.StatusCode);

        var getAfterRestore = await client.GetAsync($"/api/goals/{goal.Id}");
        Assert.Equal(HttpStatusCode.OK, getAfterRestore.StatusCode);
    }
}
