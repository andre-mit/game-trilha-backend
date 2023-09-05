using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using GameTrilha.API.Hubs;

namespace GameTrilha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LobbyController : Controller
{
    private readonly IHubContext<GameHub> _gameHubContext;

    public LobbyController(IHubContext<GameHub> gameHubContext)
    {
        _gameHubContext = gameHubContext;
    }

    [HttpGet]
    public IActionResult ListLobbies()
    {
        IGroupManager groupManager = _gameHubContext.Groups;

        var lifetimeManager = groupManager.GetType().GetRuntimeFields()
            .Single(fi => fi.Name == "_lifetimeManager")
            .GetValue(groupManager);

        var groupsObject = lifetimeManager?.GetType().GetRuntimeFields()
            .Single(fi => fi.Name == "_groups")
            .GetValue(lifetimeManager);

        var groupsDictionary = groupsObject?.GetType().GetRuntimeFields()
            .Single(fi => fi.Name == "_groups")
            .GetValue(groupsObject) as IDictionary;

        var groupNames = groupsDictionary?.Keys.Cast<string>().ToList();
        return Ok(groupNames);
    }
}