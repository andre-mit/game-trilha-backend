using GameTrilha.Domain.Entities;

namespace GameTrilha.API.Services.Interfaces;

public interface IRankingService
{
    bool AssignRankingPoints(User? userData, int scorePoints, RankingService.Action action);
}