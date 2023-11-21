using GameTrilha.Domain.Entities;
using GameTrilha.Domain.Entities.Repositories;
using GameTrilha.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GameTrilha.API.Contexts.Repositories;

public class MatchRepository : IMatchRepository
{
    private readonly TrilhaContext _context;

    public MatchRepository(TrilhaContext context)
    {
        _context = context;
    }

    public async Task<Match> Create(Guid user1Id, Guid user2Id)
    {
        var user1 = _context.Users.FirstOrDefault(x => x.Id == user1Id);
        var user2 = _context.Users.FirstOrDefault(x => x.Id == user2Id);

        if (user1 == null || user2 == null)
        {
            throw new Exception("User not found");
        }

        var match = new Match
        {
            Player1 = user1,
            Player2 = user2,
            CreatedAt = DateTime.UtcNow,
            Status = MatchStatus.InProgress
        };

        await _context.Matches.AddAsync(match);
        await _context.SaveChangesAsync();
        return match;
    }

    public async Task EndMatch(Guid matchId, Guid? winnerId = null)
    {
        var match = await _context.Matches.FirstOrDefaultAsync(x => x.Id == matchId) ?? throw new Exception("Match not found");

        if (winnerId is not null)
        {
            var winner = await _context.Users.FirstOrDefaultAsync(x => x.Id == winnerId) ?? throw new Exception("Winner not found");
            match.Winner = winner;
        }

        match.Status = MatchStatus.Finished;
        match.FinishedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}