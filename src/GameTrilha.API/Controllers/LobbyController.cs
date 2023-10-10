using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using GameTrilha.API.Hubs;
using GameTrilha.API.Services;
using GameTrilha.API.ViewModels.LobbyViewModels;

namespace GameTrilha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LobbyController : Controller
{
    private readonly IHubContext<GameHub> _gameHubContext;
    private readonly ILogger<LobbyController> _logger;

    public LobbyController(ILogger<LobbyController> logger, IHubContext<GameHub> gameHubContext)
    {
        _logger = logger;
        _gameHubContext = gameHubContext;
    }

    [HttpGet]
    public ActionResult<IEnumerable<ListLobbyViewModel>> ListLobbies()
    {
        try
        {
            var lobbies = GameService.Games.Select(game =>
                new ListLobbyViewModel(game.Key, game.Value.Players.Select(player => player.Key).ToArray(), game.Value.State));

            return Ok(lobbies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the lobbies");
            return BadRequest();
        }
    }
}