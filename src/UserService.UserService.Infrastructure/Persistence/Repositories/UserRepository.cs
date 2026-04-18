using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct) =>
        await _context.Users.AnyAsync(u => u.Name == name, ct);

    public async Task AddAsync(User user, CancellationToken ct) =>
        await _context.Users.AddAsync(user, ct);

    public async Task SaveChangesAsync(CancellationToken ct) =>
        await _context.SaveChangesAsync(ct);

    public async Task<User?> GetByNameAsync(string name, CancellationToken ct) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Name == name, ct);
}