using RoutineFlow.Models.Entities;
using RoutineFlow.Models.Enums;

namespace RoutineFlow.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetPagedAsync(Guid userId, bool? unreadOnly, int page, int pageSize);
    Task<int> CountAsync(Guid userId, bool? unreadOnly);
    Task<Notification?> GetByIdAsync(Guid userId, Guid id);
    Task MarkAllReadAsync(Guid userId);
    Task<bool> ExistsSinceAsync(Guid userId, NotificationType type, DateTime since);
    Task AddAsync(Notification notification);
    Task SaveChangesAsync();
}
