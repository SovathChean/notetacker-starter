using Microsoft.EntityFrameworkCore;
using TechbodiaNotes.Api.Infrastructure;
using TechbodiaNotes.Api.Models;

namespace TechbodiaNotes.Api.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Username == username);
    }

    public async Task<User> CreateAsync(User user)
    {
        user.Id = Guid.NewGuid();
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        var entity = await _dbContext.Users.FindAsync(user.Id);

        if (entity == null)
        {
            throw new InvalidOperationException($"User with Id {user.Id} not found");
        }

        entity.Email = user.Email;
        entity.Username = user.Username;
        entity.PasswordHash = user.PasswordHash;
        entity.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbContext.Users.FindAsync(id);
        if (entity != null)
        {
            _dbContext.Users.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
