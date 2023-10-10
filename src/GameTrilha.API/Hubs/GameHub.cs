using GameTrilha.API.Helpers;
using GameTrilha.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using GameTrilha.GameDomain.Entities;
using GameTrilha.GameDomain.Enums;
using static GameTrilha.API.Services.GameService;

namespace GameTrilha.API.Hubs;

//[Authorize]
// Todo: Remove delayed tasks
// TODO: Get ID of player by token
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
        var game = Games.FirstOrDefault(x => x.Value.Players.ContainsKey(Context.ConnectionId));

        if (game.Key == null) return base.OnDisconnectedAsync(exception);

        HandleLeave(game.Key);
        _logger.LogInformation("Client {ConnectionId} disconnected", Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }

    public async Task Join(string gameId)
    {
        if (Games[gameId].Players.Count < 2)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            await Clients.All.SendAsync("PlayerJoined", gameId, Context.ConnectionId);
            Games[gameId].Players.Add(Context.ConnectionId, new Player(false));
            _logger.LogInformation("Client {ConnectionId} tried to join game {gameId} but it was full", Context.ConnectionId, gameId);
        }
        else
        {
            await Clients.Caller.SendAsync("LobbyFull");
            _logger.LogInformation("Client {ConnectionId} tried to join game {gameId} but it was full", Context.ConnectionId, gameId);
        }
    }

    public void Leave(string gameId)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        HandleLeave(gameId);
        _logger.LogInformation("Client {ConnectionId} left game {gameId}", Context.ConnectionId, gameId);
    }

    public async Task Ready(string gameId, bool moinhoDuplo)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        Games[gameId].Players[Context.ConnectionId].Ready = !Games[gameId].Players[Context.ConnectionId].Ready;
        await Clients.OthersInGroup(gameId).SendAsync("Ready", Games[gameId].Players[Context.ConnectionId].Ready);

        _logger.LogInformation("Player {ConnectionId} is ready: {Ready}", Context.ConnectionId, GameService.Games[gameId].Players[Context.ConnectionId].Ready);
        if (Games[gameId].Players.All(player => player.Value.Ready))
        {
            Games[gameId].State = Game.GameState.Playing;
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

            await Clients.Client(player1.Key).SendAsync("StartGame", gameId, player1.Value);
            await Clients.Client(player2.Key).SendAsync("StartGame", gameId, player2.Value);
            await Clients.AllExcept(player1.Key, player2.Key).SendAsync("LobbyStarted", gameId);
            _logger.LogInformation("Game {gameId} started with players connections: {player1} and {player2}", gameId, player1.Key, player2.Key);
        }
    }

    public async Task Loaded(string gameId)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        Games[gameId].Players[Context.ConnectionId].Loaded = true;
        if (Games[gameId].Players.All(player => player.Value.Loaded))
        {
            await Clients.Group(gameId).SendAsync("PlaceStage", Games[gameId].Board!.Turn, Games[gameId].Board!.PendingPieces);
            _logger.LogInformation("Game {gameId} is in place stage by all players already loaded", gameId);
        }
    }

    public async Task Move(string gameId, int[] from, int[] to)
    {
        try
        {
            ThrowIfPlayerIsNotInGame(gameId);

            var (moinho, winner) = Games[gameId].Board!.MovePiece(Context.ConnectionId, (byte)from[0], (byte)from[1],
                (byte)from[2], (byte)to[0], (byte)to[1], (byte)to[2]);
            await Clients.Group(gameId).SendAsync("Move", from, to);
            await Task.Delay(1000);
            if (!winner.HasValue)
            {
                await Clients.Group(gameId).SendAsync("Draw");
                EndMatch(gameId);
            }
            else if (winner.Value)
            {
                await Clients.Caller.SendAsync("Win");
                await Clients.OthersInGroup(gameId).SendAsync("Lose");
                EndMatch(gameId);
            }
            else if (moinho)
            {
                await Clients.Group(gameId).SendAsync("Moinho", Games[gameId].Board!.Turn);
            }
            else
            {
                await Clients.Group(gameId).SendAsync("MoveStage", Games[gameId].Board!.Turn);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }


    public async Task Place(string gameId, int[] place)
    {
        try
        {
            ThrowIfPlayerIsNotInGame(gameId);

            var color = Games[gameId].Board!.Turn;
            var (pendingPieces, moinho, winner, pieceId) = Games[gameId].Board!.PlacePiece(Context.ConnectionId,
                (byte)place[0], (byte)place[1], (byte)place[2]);

        await Clients.Group(gameId).SendAsync("Place", pieceId, place, color, pendingPieces);

            await Task.Delay(1000);
            if (winner)
            {
                await Clients.Caller.SendAsync("Win");
                await Clients.OthersInGroup(gameId).SendAsync("Lose");
                EndMatch(gameId);
            }
            else if (moinho)
            {
                await Clients.Group(gameId).SendAsync("Moinho", Games[gameId].Board!.Turn);
            }
            else
            {
                switch (Games[gameId].Board!.Stage)
                {
                    case GameStage.Place:
                        await Clients.Group(gameId).SendAsync("PlaceStage", Games[gameId].Board!.Turn, pendingPieces);
                        break;
                    case GameStage.Game:
                        await Clients.Group(gameId).SendAsync("MoveStage", Games[gameId].Board!.Turn);
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
        ThrowIfPlayerIsNotInGame(gameId);
        var winner = Games[gameId].Board!.RemovePiece(Context.ConnectionId, (byte)place[0], (byte)place[1],
            (byte)place[2]);

        await Clients.Group(gameId).SendAsync("Remove", place);

        if (winner)
        {
            await Clients.Caller.SendAsync("Win");
            await Clients.OthersInGroup(gameId).SendAsync("Lose");
            EndMatch(gameId);
        }
        else
        {
            switch (Games[gameId].Board!.Stage)
            {
                case GameStage.Place:
                    await Clients.Group(gameId).SendAsync("PlaceStage", Games[gameId].Board!.Turn, Games[gameId].Board!.PendingPieces);
                    break;
                case GameStage.Game:
                    await Clients.Group(gameId).SendAsync("MoveStage", Games[gameId].Board!.Turn);
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
        if (!Games[gameId].Players.ContainsKey(Context.ConnectionId))
            throw new Exception("Player not in group");
    }

    private async void EndMatch(string gameId)
    {
        GameService.EndMatch(gameId);
        await Clients.All.SendAsync("GameFinished", gameId);
    }

    private async void HandleLeave(string gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);

        await Clients.All.SendAsync("PlayerLeft", gameId, Context.ConnectionId);

        switch (Games[gameId].State)
        {
            case Game.GameState.Waiting:
                Games[gameId].Players.Remove(Context.ConnectionId);
                break;
            case Game.GameState.Playing:
                await Clients.OthersInGroup(gameId).SendAsync("OpponentLeave");
                await Clients.OthersInGroup(gameId).SendAsync("Win");
                EndMatch(gameId);
                break;
            case Game.GameState.Finished:
                if (Games[gameId].Players.Count == 1)
                {
                    ResetGame(gameId);
                    await Clients.All.SendAsync("LobbyReset", gameId);
                }
                else
                {
                    await Clients.OthersInGroup(gameId).SendAsync("OpponentLeave");
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Games[gameId].Players.Remove(Context.ConnectionId);
    }
}