using RoutineFlow.Models.Entities;

namespace RoutineFlow.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    Task AddAsync(RefreshToken token);
    Task SaveChangesAsync();
}
