using Microsoft.EntityFrameworkCore;
using RoutineFlow.Data;
using RoutineFlow.Models.Entities;
using RoutineFlow.Repositories.Interfaces;

namespace RoutineFlow.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _db;

    public RefreshTokenRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash) =>
        _db.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.TokenHash == tokenHash);

    public async Task AddAsync(RefreshToken token) => await _db.RefreshTokens.AddAsync(token);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
