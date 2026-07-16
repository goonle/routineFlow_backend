using RoutineFlow.Models.Entities;

namespace RoutineFlow.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<List<Guid>> GetAllIdsAsync();
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
