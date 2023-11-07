using GameTrilha.API.Services.Interfaces;
using GameTrilha.Domain.Entities.Repositories;
using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.Services;

public class MatchService : IMatchService
{
    private readonly IMatchRepository _matchRepository;

    public MatchService(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public async Task<(KeyValuePair<Guid, Color> player1, KeyValuePair<Guid, Color> player2)> StartMatch(string gameId, bool moinhoDuplo, Guid user1Id, Guid user2Id)
    {
        var match = await _matchRepository.Create(user1Id, user2Id);
        return GameService.StartGame(gameId, moinhoDuplo, match.Id);
    }
}