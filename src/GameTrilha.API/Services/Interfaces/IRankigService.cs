using GameTrilha.Domain.Entities;

namespace GameTrilha.API.Services.Interfaces;

public interface IRankingService
{
    Task<bool> AssignRankingPointsAsync(Guid userId, int scorePoints, RankingService.Action action);
}