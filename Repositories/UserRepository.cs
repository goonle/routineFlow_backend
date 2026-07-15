using Microsoft.EntityFrameworkCore;
using RoutineFlow.Data;
using RoutineFlow.Models.Entities;
using RoutineFlow.Repositories.Interfaces;

namespace RoutineFlow.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;

    public UserRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByEmailAsync(string email) =>
        _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetByIdAsync(Guid id) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task AddAsync(User user) => await _db.Users.AddAsync(user);

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}
