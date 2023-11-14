using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.Services.Interfaces;

public interface IMatchService
{
    Task<(KeyValuePair<Guid, Color> player1, KeyValuePair<Guid, Color> player2)> StartMatch(string gameId, Guid user1Id, Guid user2Id);
}