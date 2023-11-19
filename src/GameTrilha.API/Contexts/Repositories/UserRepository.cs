using GameTrilha.API.Controllers;
using GameTrilha.Domain.Entities;
using GameTrilha.Domain.Entities.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GameTrilha.API.Contexts.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TrilhaContext _context;

    public UserRepository(TrilhaContext context)
    {
        _context = context;
    }

    public async Task<User> Create(string name, string email, string password)
    {
        if (await _context.Users.AnyAsync(x => x.Email == email))
        {
            throw new Exception("Email already exists");
        }

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == "User");

        var user = new User(name, email, password);
        user.Roles = new List<UserRole> { new() { Role = role, User = user } };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public User Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
        return user;
    }

    public async Task<bool> Delete(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

        if (user == null) return false;

        _context.Users.Remove(user);
        var result = await _context.SaveChangesAsync();
        return result > 0;

    }

    public async Task<User?> FindById(Guid id)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(x => x.Roles)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User?> FindByEmail(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == email);
    }
    public async Task<List<Skin>?> ListSkins(Guid id)
    {
        return await _context.Skins
           .AsNoTracking()
           .Where(x => x.Users.Select(u => u.Id).Contains(id))
           .ToListAsync();
    }

    public async Task<List<Board>?> ListBoards(Guid id)
    {
        return await _context.Boards
           .AsNoTracking()
           .Where(x => x.Users.Select(u => u.Id).Contains(id))
           .ToListAsync();
    }

    public async Task<List<Skin>?> ListSkinsRemaining(Guid id)
    {
        return await _context.Skins
           .AsNoTracking()
           .Where(x => !x.Users.Select(u => u.Id).Contains(id))
           .ToListAsync();
    }

    public async Task<List<Board>?> ListBoardsRemaining(Guid id)
    {
        return await _context.Boards
            .AsNoTracking()
            .Where(x => !x.Users.Select(u => u.Id).Contains(id))
            .ToListAsync();
    }

}