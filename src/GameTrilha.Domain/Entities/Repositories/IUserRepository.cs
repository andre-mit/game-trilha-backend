using GameTrilha.Domain.DTOs.Users;
using GameTrilha.Domain.ValueObjects;

namespace GameTrilha.Domain.Entities.Repositories;

public interface IUserRepository
{
    Task<User> Create(string name, string email, string password, UserAvatar avatar);
    User Update(User user);
    Task<bool> Delete(Guid id);
    Task<User?> FindById(Guid id);
    Task<UserSimpleProfile?> GetSimpleProfileByIdAsync(Guid id);
    Task<List<UserSimpleProfile>> GetSimpleProfileByIdsAsync(Guid[] id);
    Task<User?> FindByEmail(string email);
    Task<RecoveryPasswordCode> CreateRecoveryPasswordAsync(Guid userId, string code, DateTime expiresAt);
    Task<bool> UseRecoveryPasswordCodeAsync(string email, string code, string newPassword);
    Task<List<Skin>?> ListSkins(Guid id);
    Task<List<Board>?> ListBoards(Guid id);
    Task<List<Skin>?> ListSkinsRemaining(Guid id);
    Task<List<Board>?> ListBoardsRemaining(Guid id);
    Task<(Guid? selectedSkin, Guid? selectedBoard)> GetSelectedSkinAndBoard(Guid userId);
    Task<bool> IncreaseScoreAsync(Guid id, int score);
    Task<bool> DecreaseScoreAsync(Guid id, int score);
}