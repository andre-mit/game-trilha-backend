using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.Contexts;
using GameTrilha.API.ViewModels.UserViewModels;
using GameTrilha.Domain.Entities;
using GameTrilha.Domain.Entities.Repositories;
using GameTrilha.API.Contexts.Repositories;

namespace GameTrilha.API.Services;

public class RankingService : IRankingService
{
    private readonly IUserRepository _userRepository;

    public RankingService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public enum Action
    {
        Decrease,
        Increase
    }

    public async Task<bool> AssignRankingPointsAsync(Guid userId, int scorePoints, Action action)
    {
        var success = action switch
        {
            Action.Decrease => await _userRepository.DecreaseScoreAsync(userId, scorePoints),
            Action.Increase => await _userRepository.IncreaseScoreAsync(userId, scorePoints),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null),
        };

        return success;
    }
}