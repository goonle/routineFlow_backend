namespace RoutineFlow.DTOs.Notifications;

public class NotificationListResponse
{
    public List<NotificationResponse> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}
