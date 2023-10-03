using GameTrilha.API.Helpers;
using GameTrilha.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using GameTrilha.GameDomain.Entities;
using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.Hubs;

//[Authorize]
// Todo: Remove delayed tasks
public class GameHub : Hub
{
    private readonly ILogger<GameHub> _logger;

    public GameHub(ILogger<GameHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        _logger.LogInformation("Client {ConnectionId} connected", Context.ConnectionId);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var game = GameService.Games.FirstOrDefault(x => x.Value.Players.ContainsKey(Context.ConnectionId));

        if (game.Key == null) return base.OnDisconnectedAsync(exception);

        GameService.Games[game.Key].Players.Remove(Context.ConnectionId);
        Clients.All.SendAsync("PlayerLeft", game.Key, Context.ConnectionId);

        _logger.LogInformation("Client {ConnectionId} disconnected", Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }

    public async Task Join(string gameId)
    {
        if (GameService.Games[gameId].Players.Count < 2)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.All.SendAsync("PlayerJoined", gameId, Context.ConnectionId);
            GameService.Games[gameId].Players.Add(Context.ConnectionId, new GameService.Player(false));

            _logger.LogInformation("Client {ConnectionId} joined game {gameId}", Context.ConnectionId, gameId);
        }
        else
        {
            await Clients.Caller.SendAsync("LobbyFull");
            _logger.LogInformation("Client {ConnectionId} tried to join game {gameId} but it was full", Context.ConnectionId, gameId);
        }
    }

    public async Task Leave(string gameId)
    {
        ThrowIfPlayerIsNotInGame(gameId);

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        await Clients.All.SendAsync("PlayerLeft", gameId, Context.ConnectionId);
        GameService.Games[gameId].Players.Remove(Context.ConnectionId);
        _logger.LogInformation("Client {ConnectionId} left game {gameId}", Context.ConnectionId, gameId);
    }

    public async Task Ready(string gameId, bool moinhoDuplo)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        GameService.Games[gameId].Players[Context.ConnectionId].Ready = !GameService.Games[gameId].Players[Context.ConnectionId].Ready;
        await Clients.OthersInGroup(gameId).SendAsync("Ready", GameService.Games[gameId].Players[Context.ConnectionId].Ready);

        _logger.LogInformation("Player {ConnectionId} is ready: {Ready}", Context.ConnectionId, GameService.Games[gameId].Players[Context.ConnectionId].Ready);

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
            _logger.LogInformation("Game {gameId} started with players connections: {player1} and {player2}", gameId, player1.Key, player2.Key);
        }
    }

    public async Task Loaded(string gameId)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        GameService.Games[gameId].Players[Context.ConnectionId].Loaded = true;
        _logger.LogInformation("Player {ConnectionId} is loaded", Context.ConnectionId);

        if (GameService.Games[gameId].Players.All(player => player.Value.Loaded))
        {
            await Clients.Group(gameId).SendAsync("PlaceStage", GameService.Games[gameId].Board!.Turn, GameService.Games[gameId].Board!.PendingPieces);
            _logger.LogInformation("Game {gameId} is in place stage by all players already loaded", gameId);
        }
    }

    public async Task Move(string gameId, int[] from, int[] to)
    {
        try
        {
            ThrowIfPlayerIsNotInGame(gameId);

            var (moinho, winner) = GameService.Games[gameId].Board!.MovePiece(Context.ConnectionId, (byte)from[0], (byte)from[1], (byte)from[2], (byte)to[0], (byte)to[1], (byte)to[2]);
            await Clients.Group(gameId).SendAsync("Move", from, to);
            _logger.LogInformation("Player {ConnectionId} moved piece from {@from} to {@to}", Context.ConnectionId, from, to);

            await Task.Delay(1000);

            if (!winner.HasValue)
            {
                await Clients.Group(gameId).SendAsync("Draw");
                _logger.LogInformation("Game {gameId} ended in a draw", gameId);
            }
            else if (winner.Value)
            {
                await Clients.Caller.SendAsync("Win");
                await Clients.OthersInGroup(gameId).SendAsync("Lose", Context.ConnectionId);
                _logger.LogInformation("Player {ConnectionId} won game {gameId}", Context.ConnectionId, gameId);
            }
            else if (moinho)
            {
                await Clients.Group(gameId).SendAsync("Moinho", GameService.Games[gameId].Board!.Turn);
                _logger.LogInformation("Player {ConnectionId} made a moinho", Context.ConnectionId);
            }
            else
            {
                await Clients.Group(gameId).SendAsync("MoveStage", GameService.Games[gameId].Board!.Turn);
                _logger.LogInformation("Game {gameId} is in move stage", gameId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving piece");
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }


    public async Task Place(string gameId, int[] place)
    {
        try
        {
            ThrowIfPlayerIsNotInGame(gameId);

            var color = GameService.Games[gameId].Board!.Turn;
            var (pendingPieces, moinho, winner, pieceId) = GameService.Games[gameId].Board!.PlacePiece(Context.ConnectionId, (byte)place[0], (byte)place[1], (byte)place[2]);

            await Clients.Group(gameId).SendAsync("Place", pieceId, place, color, pendingPieces);

            _logger.LogInformation("Player {ConnectionId} placed piece {@place}", Context.ConnectionId, place);

            await Task.Delay(1000);

            if (winner)
            {
                await Clients.Caller.SendAsync("Win");
                await Clients.OthersInGroup(gameId).SendAsync("Lose", Context.ConnectionId);
                _logger.LogInformation("Player {ConnectionId} won game {gameId}", Context.ConnectionId, gameId);
            }
            else if (moinho)
            {
                await Clients.Group(gameId).SendAsync("Moinho", GameService.Games[gameId].Board!.Turn);
                _logger.LogInformation("Player {ConnectionId} made a moinho", Context.ConnectionId);
            }
            else
            {
                switch (GameService.Games[gameId].Board!.Stage)
                {
                    case GameStage.Place:
                        await Clients.Group(gameId).SendAsync("PlaceStage", GameService.Games[gameId].Board!.Turn, pendingPieces);
                        _logger.LogInformation("Game {gameId} is in place stage", gameId);
                        break;
                    case GameStage.Game:
                        await Clients.Group(gameId).SendAsync("MoveStage", GameService.Games[gameId].Board!.Turn);
                        _logger.LogInformation("Game {gameId} is in move stage", gameId);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error placing piece");
            await Clients.Caller.SendAsync("Error", ex.Message);
        }
    }

    public async Task Remove(string gameId, int[] place)
    {
        try
        {
            ThrowIfPlayerIsNotInGame(gameId);
            var winner = GameService.Games[gameId].Board!.RemovePiece(Context.ConnectionId, (byte)place[0], (byte)place[1],
                (byte)place[2]);

            await Clients.Group(gameId).SendAsync("Remove", place);

            _logger.LogInformation("Player {ConnectionId} removed piece {@place}", Context.ConnectionId, place);

            if (winner)
            {
                await Clients.Caller.SendAsync("Win");
                await Clients.OthersInGroup(gameId).SendAsync("Lose", Context.ConnectionId);
                _logger.LogInformation("Player {ConnectionId} won game {gameId}", Context.ConnectionId, gameId);
            }
            else
            {
                switch (GameService.Games[gameId].Board!.Stage)
                {
                    case GameStage.Place:
                        await Clients.Group(gameId).SendAsync("PlaceStage", GameService.Games[gameId].Board!.Turn, GameService.Games[gameId].Board!.PendingPieces);
                        _logger.LogInformation("Game {gameId} is in place stage", gameId);
                        break;
                    case GameStage.Game:
                        await Clients.Group(gameId).SendAsync("MoveStage", GameService.Games[gameId].Board!.Turn);
                        _logger.LogInformation("Game {gameId} is in move stage", gameId);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing piece");
            await Clients.Caller.SendAsync("Error", ex.Message);
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