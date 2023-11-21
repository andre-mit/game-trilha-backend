using GameTrilha.API.Services.Interfaces;
using GameTrilha.Domain.Entities.Repositories;

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

    public async Task EndMatch(string gameId, Guid? winnerId = null)
    {
        var matchId = GameService.GetMatchId(gameId);
        await _matchRepository.EndMatch(matchId, winnerId);
        GameService.EndMatch(gameId);
    }
}