using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using GameTrilha.GameDomain.Entities;
using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.Hubs;

//[Authorize]
public class GameHub : Hub
{
    public async Task Join(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        await Clients.OthersInGroup(gameId).SendAsync("JoinGame");
    }

    public async Task Leave(string gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        await Clients.OthersInGroup(gameId).SendAsync("LeaveGame");
    }

    public async Task Ready(string gameId)
    {
        await Clients.OthersInGroup(gameId).SendAsync("Ready");
    }

    public async Task Start(string gameId)
    {
        var pieces = Enumerable.Range(1, 18).Select(_ => new Piece(_ < 10 ? Color.White : Color.Black)).GroupBy(x => x.Color).ToList();
        await Clients.Group(gameId).SendAsync("Start", pieces);
    }

    public async Task Move(string gameId, int[] from, int[] to)
    {
        await Clients.Group(gameId).SendAsync("Move", from, to);
    }

    public async Task Place(string gameId, int[] to)
    {
        await Clients.Group(gameId).SendAsync("Place", to);
    }

    public async Task Remove(string gameId, int[] from)
    {
        await Clients.Group(gameId).SendAsync("Remove", from);
    }

    public async Task EndGame(string gameId)
    {
        await Clients.Group(gameId).SendAsync("EndGame");
    }

    public async Task Rematch(string gameId)
    {
        await Clients.Group(gameId).SendAsync("Rematch");
    }
}