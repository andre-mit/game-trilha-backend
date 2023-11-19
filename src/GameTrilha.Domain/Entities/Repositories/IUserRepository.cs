namespace GameTrilha.Domain.Entities.Repositories;

public interface IUserRepository
{
    Task<User> Create(string name, string email, string password);
    User Update(User user);
    Task<bool> Delete(Guid id);
    Task<User?> FindById(Guid id);
    Task<User?> FindByEmail(string email);
    Task<List<Skin>?> ListSkins(Guid id);
    Task<List<Board>?> ListBoards(Guid id);
    Task<List<Skin>?> ListSkinsRemaining(Guid id);
    Task<List<Board>?> ListBoardsRemaining(Guid id);
}