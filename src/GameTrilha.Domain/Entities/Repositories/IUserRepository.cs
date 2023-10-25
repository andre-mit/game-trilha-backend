namespace GameTrilha.Domain.Entities.Repositories;

public interface IUserRepository
{
    Task<User> Create(string name, string email, string password);
    User Update(User user);
    User Delete(User user);
    Task<User> FindById(Guid id);
    User FindByEmail(string email);
}