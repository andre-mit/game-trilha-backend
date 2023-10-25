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

        var user = new User(name, email, password);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public User Update(User user)
    {
        throw new NotImplementedException();
    }

    public User Delete(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User> FindById(Guid id)
    {
        throw new NotImplementedException();
    }

    public User FindByEmail(string email)
    {
        throw new NotImplementedException();
    }
}