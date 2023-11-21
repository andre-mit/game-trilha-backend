using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.Contexts;
using GameTrilha.API.ViewModels.UserViewModels;
using GameTrilha.Domain.Entities;
using GameTrilha.Domain.Entities.Repositories;
using GameTrilha.API.Contexts.Repositories;

namespace GameTrilha.API.Services;

public class RankingService : IRankingService
{
    private readonly TrilhaContext _trilhaContext;
    private readonly IUserRepository _userRepository;

    public RankingService(TrilhaContext context, IUserRepository userRepository)
    {
        _trilhaContext = context;
        _userRepository = userRepository;
    }

    public enum Action
    {
        Decrease,
        Increase
    }

    public bool AssignRankingPoints(User? userData, int scorePoints, Action action)
    {
        if (userData is null)
        {
            return false;
        }

        UpdatePlayerRankingPoints(userData, scorePoints, action);
        SaveUpdate();

        return true;
    }

    private void UpdatePlayerRankingPoints(User userData, int scorePoints, Action action)
    {
        if (action == Action.Increase) { userData.AddScore(scorePoints); }
        if (action == Action.Decrease) { userData.RemoveScore(scorePoints); }
    }

        private void SaveUpdate()
    {
        _trilhaContext.SaveChanges();
    }
}