using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using GameTrilha.API.Hubs;
using GameTrilha.API.Services;
using GameTrilha.API.ViewModels.LobbyViewModels;
using GameTrilha.Domain.Entities.Repositories;

namespace GameTrilha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LobbyController : ControllerBase
{
    private readonly ILogger<LobbyController> _logger;
    private readonly IHubContext<GameHub> _gameHubContext;
    private readonly IUserRepository _userRepository;

    public LobbyController(ILogger<LobbyController> logger, IHubContext<GameHub> gameHubContext, IUserRepository userRepository)
    {
        _logger = logger;
        _gameHubContext = gameHubContext;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ListLobbyViewModel>>> ListLobbies()
    {
        try
        {
            var lobbies = GameService.Games;
            var usersIds = lobbies.Select(x => x.Value.Players).SelectMany(x => x.Keys).Distinct().ToArray();
            var users = await _userRepository.GetSimpleProfileByIdsAsync(usersIds);

            List<ListLobbyViewModel> viewModel = (from lobby in lobbies
                let lobbyUsers = users.Where(x => lobby.Value.Players.ContainsKey(x.Id))
                    .Select(x => new ListUserProfileLobby(x.Id, x.Name, x.Avatar, lobby.Value.Players.First(p => p.Key == x.Id).Value.Moinho))
                    .ToArray()
                select new ListLobbyViewModel(lobby.Key, lobbyUsers, lobby.Value.State)).ToList();


            return Ok(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the lobbies");
            return BadRequest();
        }
    }
}