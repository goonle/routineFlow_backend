using RoutineFlow.DTOs.Notifications;

namespace RoutineFlow.Services.Interfaces;

public interface INotificationService
{
    Task<NotificationListResponse> GetAsync(Guid userId, bool? unreadOnly, int page, int pageSize);
    Task<NotificationResponse> MarkReadAsync(Guid userId, Guid id);
    Task MarkAllReadAsync(Guid userId);
}
