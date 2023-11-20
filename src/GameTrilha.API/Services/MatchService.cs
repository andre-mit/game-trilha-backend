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

    public async Task StartMatch(string gameId, Guid user1Id, Guid user2Id)
    {
        var match = await _matchRepository.Create(user1Id, user2Id);
        GameService.StartGame(gameId, match.Id);
    }
}