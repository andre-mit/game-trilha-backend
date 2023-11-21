using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.Services.Interfaces;

public interface IMatchService
{
    Task StartMatch(string gameId, Guid user1Id, Guid user2Id);
    Task EndMatch(string gameId, Guid? winnerId = null);
}