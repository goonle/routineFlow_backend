using RoutineFlow.Common.Exceptions;
using RoutineFlow.DTOs.Notifications;
using RoutineFlow.Models.Entities;
using RoutineFlow.Repositories.Interfaces;
using RoutineFlow.Services.Interfaces;

namespace RoutineFlow.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<NotificationListResponse> GetAsync(Guid userId, bool? unreadOnly, int page, int pageSize)
    {
        if (page < 1)
        {
            throw new ArgumentException("page must be at least 1.");
        }

        if (pageSize is < 1 or > 100)
        {
            throw new ArgumentException("pageSize must be between 1 and 100.");
        }

        var items = await _notificationRepository.GetPagedAsync(userId, unreadOnly, page, pageSize);
        var totalCount = await _notificationRepository.CountAsync(userId, unreadOnly);

        return new NotificationListResponse
        {
            Items = items.Select(ToResponse).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<NotificationResponse> MarkReadAsync(Guid userId, Guid id)
    {
        var notification = await _notificationRepository.GetByIdAsync(userId, id);
        if (notification is null)
        {
            throw new NotFoundException("Notification not found.");
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await _notificationRepository.SaveChangesAsync();
        }

        return ToResponse(notification);
    }

    public Task MarkAllReadAsync(Guid userId) => _notificationRepository.MarkAllReadAsync(userId);

    private static NotificationResponse ToResponse(Notification notification) => new()
    {
        Id = notification.Id,
        Type = notification.Type,
        Content = notification.Content,
        IsRead = notification.IsRead,
        CreatedAt = notification.CreatedAt
    };
}
