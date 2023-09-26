using System.Collections.Concurrent;
using GameTrilha.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using GameTrilha.GameDomain.Entities;
using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.Hubs;

//[Authorize]
public class GameHub : Hub
{
    private static readonly ConcurrentDictionary<string, Game> Games = new(Enumerable.Range(1, 20).ToDictionary(x => x.ToString(), x => new Game()));

    public class Game
    {
        public Dictionary<string, Player> Players { get; set; }
        public Board? Board { get; set; }
        public bool Started { get; set; }

        public Game()
        {
            Players = new Dictionary<string, Player>();
            Started = false;
            Board = null;
        }
    }

    public class Player
    {
        public bool Ready { get; set; }

        public Player(bool ready)
        {
            Ready = ready;
        }
    }


    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId, Games);
    }

    public async Task Join(string gameId)
    {
        if (Games[gameId].Players.Count < 2)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.All.SendAsync("PlayerJoined", gameId, Context.ConnectionId);
            Games[gameId].Players.Add(Context.ConnectionId, new Player(false));

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
        Games[gameId].Players.Remove(Context.ConnectionId);
    }

    public async Task Ready(string gameId, bool moinhoDuplo)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        Games[gameId].Players[Context.ConnectionId].Ready = !Games[gameId].Players[Context.ConnectionId].Ready;
        await Clients.OthersInGroup(gameId).SendAsync("Ready", Games[gameId].Players[Context.ConnectionId].Ready);

        if (Games[gameId].Players.All(player => player.Value.Ready))
        {
            Games[gameId].Started = true;
            var player1 =
                new KeyValuePair<string, Color>(Games[gameId].Players.ElementAt(0).Key, RandomColor.GetRandomColor());
            var player2 =
                new KeyValuePair<string, Color>(Games[gameId].Players.ElementAt(1).Key, RandomColor.GetOppositeColor(player1.Value));

            var players = new Dictionary<string, Color>
            {
                { player1.Key, player1.Value },
                { player2.Key, player2.Value }
            };


            Games[gameId].Board = new Board(moinhoDuplo, players);
            await Clients.Group(gameId).SendAsync("StartGame", gameId);
        }
    }

    public async Task Move(string gameId, int[] from, int[] to)
    {
        ThrowIfPlayerIsNotInGame(gameId);

        var (moinho, winner) = Games[gameId].Board!.MovePiece(Context.ConnectionId, (byte)from[0], (byte)from[1], (byte)from[2], (byte)to[0], (byte)to[1], (byte)to[2]);
        await Clients.Group(gameId).SendAsync("Move", from, to);

        if (!winner.HasValue)
        {
            await Clients.Group(gameId).SendAsync("Draw");
        }
        else if (winner.Value)
        {
            await Clients.Caller.SendAsync("Winner");
            await Clients.OthersInGroup(gameId).SendAsync("Loss", Context.ConnectionId);
        }
        else if (moinho)
        {
            await Clients.Group(gameId).SendAsync("Moinho", Games[gameId].Board!.Turn);
        }
    }


    public async Task Place(string gameId, byte[] place)
    {
        ThrowIfPlayerIsNotInGame(gameId);

        var color = Games[gameId].Board!.Turn;
        var (pendingPieces, moinho, winner) = Games[gameId].Board!.PlacePiece(Context.ConnectionId, place[0], place[1], place[2]);

        await Clients.Group(gameId).SendAsync("Place", place, color, pendingPieces);

        if (winner)
        {
            await Clients.Caller.SendAsync("Winner");
            await Clients.OthersInGroup(gameId).SendAsync("Loss", Context.ConnectionId);
        }
        else if (moinho)
        {
            await Clients.Group(gameId).SendAsync("Moinho", Games[gameId].Board!.Turn);
        }
    }

    public async Task Remove(string gameId, byte[] place)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        await Clients.Group(gameId).SendAsync("Remove", place);
    }

    public async Task Rematch(string gameId)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        await Clients.Group(gameId).SendAsync("Rematch");
    }

    private void ThrowIfPlayerIsNotInGame(string gameId)
    {
        if (!Games[gameId].Players.ContainsKey(Context.ConnectionId))
        {
            throw new Exception("Player not in group");

        }
    }
}