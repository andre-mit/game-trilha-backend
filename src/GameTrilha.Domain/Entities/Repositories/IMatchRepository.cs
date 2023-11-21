namespace GameTrilha.Domain.Entities.Repositories;

public interface IMatchRepository
{
    Task<Match> Create(Guid user1Id, Guid user2Id);
    Task EndMatch(Guid matchId, Guid? winnerId = null);
}