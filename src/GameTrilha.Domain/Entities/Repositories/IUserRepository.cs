using GameTrilha.Domain.ValueObjects;

namespace GameTrilha.Domain.Entities.Repositories;

public interface IUserRepository
{
    Task<User> Create(string name, string email, string password, UserAvatar avatar);
    User Update(User user);
    Task<bool> Delete(Guid id);
    Task<User?> FindById(Guid id);
    Task<User?> FindByEmail(string email);
}