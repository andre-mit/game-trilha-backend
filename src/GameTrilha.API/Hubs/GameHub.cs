using System.Collections.Concurrent;
using GameTrilha.API.Helpers;
using GameTrilha.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using GameTrilha.GameDomain.Entities;
using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.Hubs;

//[Authorize]
public class GameHub : Hub
{

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var game = GameService.Games.FirstOrDefault(x => x.Value.Players.ContainsKey(Context.ConnectionId));

        if (game.Key == null) return base.OnDisconnectedAsync(exception);

        GameService.Games[game.Key].Players.Remove(Context.ConnectionId);
        Clients.All.SendAsync("PlayerLeft", game.Key, Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task Join(string gameId)
    {
        if (GameService.Games[gameId].Players.Count < 2)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.All.SendAsync("PlayerJoined", gameId, Context.ConnectionId);
            GameService.Games[gameId].Players.Add(Context.ConnectionId, new GameService.Player(false));

            //if (Games[gameId].Players.Count == 2)
            //{
            //    var players = Games[gameId].Players.Select((player, index) => new KeyValuePair<string, Color>(player.Key, index == 0 ? Color.White : Color.Black));
            //    Games[gameId].Board = new Board(false, new Dictionary<string, Color>(players));
            //}
        }
        else
        {
            await Clients.Caller.SendAsync("LobbyFull");
        }
    }

    public async Task Leave(string gameId)
    {
        ThrowIfPlayerIsNotInGame(gameId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        await Clients.All.SendAsync("PlayerLeft", gameId, Context.ConnectionId);
        GameService.Games[gameId].Players.Remove(Context.ConnectionId);
    }

    public async Task Ready(string gameId, bool moinhoDuplo)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        GameService.Games[gameId].Players[Context.ConnectionId].Ready = !GameService.Games[gameId].Players[Context.ConnectionId].Ready;
        await Clients.OthersInGroup(gameId).SendAsync("Ready", GameService.Games[gameId].Players[Context.ConnectionId].Ready);

        if (GameService.Games[gameId].Players.All(player => player.Value.Ready))
        {
            GameService.Games[gameId].Started = true;
            var player1 =
                new KeyValuePair<string, Color>(GameService.Games[gameId].Players.ElementAt(0).Key, RandomColor.GetRandomColor());
            var player2 =
                new KeyValuePair<string, Color>(GameService.Games[gameId].Players.ElementAt(1).Key, RandomColor.GetOppositeColor(player1.Value));

            var players = new Dictionary<string, Color>
            {
                { player1.Key, player1.Value },
                { player2.Key, player2.Value }
            };


            GameService.Games[gameId].Board = new Board(moinhoDuplo, players);

            await Clients.Client(player1.Key).SendAsync("StartGame", gameId, player1.Value);
            await Clients.Client(player2.Key).SendAsync("StartGame", gameId, player2.Value);

            await Task.Delay(2000);

            await Clients.Group(gameId).SendAsync("PlaceStage", GameService.Games[gameId].Board!.Turn, GameService.Games[gameId].Board!.PendingPieces);
        }
    }

    public async Task Move(string gameId, int[] from, int[] to)
    {
        ThrowIfPlayerIsNotInGame(gameId);

        var (moinho, winner) = GameService.Games[gameId].Board!.MovePiece(Context.ConnectionId, (byte)from[0], (byte)from[1], (byte)from[2], (byte)to[0], (byte)to[1], (byte)to[2]);
        await Clients.Group(gameId).SendAsync("Move", from, to);
        await Task.Delay(1000);
        if (!winner.HasValue)
        {
            await Clients.Group(gameId).SendAsync("Draw");
        }
        else if (winner.Value)
        {
            await Clients.Caller.SendAsync("Win");
            await Clients.OthersInGroup(gameId).SendAsync("Loss", Context.ConnectionId);
        }
        else if (moinho)
        {
            await Clients.Group(gameId).SendAsync("Moinho", GameService.Games[gameId].Board!.Turn);
        }
        else
        {
            await Clients.Group(gameId).SendAsync("MoveStage", GameService.Games[gameId].Board!.Turn);
        }
    }


    public async Task Place(string gameId, int[] place)
    {
        ThrowIfPlayerIsNotInGame(gameId);

        var color = GameService.Games[gameId].Board!.Turn;
        var (pendingPieces, moinho, winner, pieceId) = GameService.Games[gameId].Board!.PlacePiece(Context.ConnectionId, (byte)place[0], (byte)place[1], (byte)place[2]);

        await Clients.Group(gameId).SendAsync("Place", pieceId, place, color, pendingPieces);

        await Task.Delay(1000);
        if (winner)
        {
            await Clients.Caller.SendAsync("Win");
            await Clients.OthersInGroup(gameId).SendAsync("Loss", Context.ConnectionId);
        }
        else if (moinho)
        {
            await Clients.Group(gameId).SendAsync("Moinho", GameService.Games[gameId].Board!.Turn);
        }
        else
        {
            switch (GameService.Games[gameId].Board!.Stage)
            {
                case GameStage.Place:
                    await Clients.Group(gameId).SendAsync("PlaceStage", GameService.Games[gameId].Board!.Turn, pendingPieces);
                    break;
                case GameStage.Game:
                    await Clients.Group(gameId).SendAsync("MoveStage", GameService.Games[gameId].Board!.Turn);
                    break;
            }
        }
    }

    public async Task Remove(string gameId, int[] place)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        var winner = GameService.Games[gameId].Board!.RemovePiece(Context.ConnectionId, (byte)place[0], (byte)place[1],
            (byte)place[2]);

        await Clients.Group(gameId).SendAsync("Remove", place);

        if (winner)
        {
            await Clients.Caller.SendAsync("Win");
            await Clients.OthersInGroup(gameId).SendAsync("Loss", Context.ConnectionId);
        }
        else
        {
            switch (GameService.Games[gameId].Board!.Stage)
            {
                case GameStage.Place:
                    await Clients.Group(gameId).SendAsync("PlaceStage", GameService.Games[gameId].Board!.Turn, GameService.Games[gameId].Board!.PendingPieces);
                    break;
                case GameStage.Game:
                    await Clients.Group(gameId).SendAsync("MoveStage", GameService.Games[gameId].Board!.Turn);
                    break;
            }
        }
    }

    public async Task Rematch(string gameId)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        await Clients.Group(gameId).SendAsync("Rematch");
    }

    private void ThrowIfPlayerIsNotInGame(string gameId)
    {
        if (!GameService.Games[gameId].Players.ContainsKey(Context.ConnectionId))
        {
            throw new Exception("Player not in group");

        }
    }
}