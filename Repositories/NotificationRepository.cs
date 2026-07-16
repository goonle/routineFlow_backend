using Microsoft.EntityFrameworkCore;
using RoutineFlow.Data;
using RoutineFlow.Models.Entities;
using RoutineFlow.Models.Enums;
using RoutineFlow.Repositories.Interfaces;

namespace RoutineFlow.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _db;

    public NotificationRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<List<Notification>> GetPagedAsync(Guid userId, bool? unreadOnly, int page, int pageSize)
    {
        var query = Filter(_db.Notifications.Where(n => n.UserId == userId), unreadOnly);
        return query.OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task<int> CountAsync(Guid userId, bool? unreadOnly) =>
        Filter(_db.Notifications.Where(n => n.UserId == userId), unreadOnly).CountAsync();

    public Task<Notification?> GetByIdAsync(Guid userId, Guid id) =>
        _db.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

    public async Task MarkAllReadAsync(Guid userId)
    {
        await _db.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
    }

    public Task<bool> ExistsSinceAsync(Guid userId, NotificationType type, DateTime since) =>
        _db.Notifications.AnyAsync(n => n.UserId == userId && n.Type == type && n.CreatedAt >= since);

    public async Task AddAsync(Notification notification) => await _db.Notifications.AddAsync(notification);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();

    private static IQueryable<Notification> Filter(IQueryable<Notification> query, bool? unreadOnly) =>
        unreadOnly == true ? query.Where(n => !n.IsRead) : query;
}
