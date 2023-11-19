using GameTrilha.API.Helpers;
using GameTrilha.API.Services;
using GameTrilha.API.Services.Interfaces;
using GameTrilha.Domain.Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using GameTrilha.GameDomain.Entities;
using GameTrilha.GameDomain.Enums;
using static GameTrilha.API.Services.GameService;

namespace GameTrilha.API.Hubs;

[Authorize]
// Todo: Remove delayed tasks
// TODO: Get ID of player by token (Context.UserIdentifier), after JWT implementation
public class GameHub : Hub
{
    private Guid UserId => Guid.Parse(Context.UserIdentifier!);
    private readonly IMatchService _matchService;
    private readonly IUserRepository _userRepository;

    public GameHub(IMatchService matchService, IUserRepository userRepository)
    {
        _matchService = matchService;
        _userRepository = userRepository;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.Caller.SendAsync("Connected", UserId);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var game = Games.FirstOrDefault(x => x.Value.Players.ContainsKey(UserId));

        if (game.Key == null) return base.OnDisconnectedAsync(exception);

        HandleLeave(game.Key);

        return base.OnDisconnectedAsync(exception);
    }

    public async Task Join(string gameId)
    {
        try
        {
            if (Games[gameId].Players.Count < 2)
            {
                var user = await _userRepository.GetSimpleProfileByIdAsync(UserId);
                await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
                await Clients.All.SendAsync("PlayerJoined", gameId, user);
                Games[gameId].Players.Add(UserId, new Player(Context.ConnectionId, false));
            }
            else
            {
                await Clients.Caller.SendAsync("LobbyFull");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public async Task ToggleMoinho(string gameId, bool moinho)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        Games[gameId].Players[UserId].Moinho = moinho;
        await Clients.All.SendAsync("ToggleMoinho", gameId, UserId, moinho);
    }

    public void Leave(string gameId)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        HandleLeave(gameId);
    }

    public async Task Ready(string gameId)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        Games[gameId].Players[UserId].Ready = !Games[gameId].Players[UserId].Ready;
        await Clients.OthersInGroup(gameId).SendAsync("Ready", Games[gameId].Players[UserId].Ready);

        var players = Games[gameId].Players;
        if (players.All(player => player.Value.Ready))
        {
            await _matchService.StartMatch(gameId, Games[gameId].Players.ElementAt(0).Key,
                Games[gameId].Players.ElementAt(1).Key);
            
            foreach (var connectionId in players.Select(p => p.Value.ConnectionId))
            {
                await Clients.Client(connectionId).SendAsync("StartMatch");
            }

            await Clients.AllExcept(players.Select(p => p.Value.ConnectionId)).SendAsync("LobbyStarted", gameId);

            await Task.Delay(5000).ContinueWith(async _ => await Clients.Group(gameId).SendAsync("PlaceStage", Games[gameId].Board!.Turn, Games[gameId].Board!.PendingPieces));
        }
    }

    public async Task Loaded(string gameId)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        Games[gameId].Players[UserId].Loaded = true;
        if (Games[gameId].Players.All(player => player.Value.Loaded))
        {
        }
    }

    public async Task Move(string gameId, int[] from, int[] to)
    {
        ThrowIfPlayerIsNotInGame(gameId);

        var (moinho, winner) = Games[gameId].Board!.MovePiece(UserId, (byte)from[0], (byte)from[1], (byte)from[2], (byte)to[0], (byte)to[1], (byte)to[2]);
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


    public async Task Place(string gameId, int[] place)
    {
        ThrowIfPlayerIsNotInGame(gameId);

        var color = Games[gameId].Board!.Turn;
        var (pendingPieces, moinho, winner, pieceId) = Games[gameId].Board!.PlacePiece(UserId, (byte)place[0], (byte)place[1], (byte)place[2]);

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

    public async Task Remove(string gameId, int[] place)
    {
        ThrowIfPlayerIsNotInGame(gameId);
        var winner = Games[gameId].Board!.RemovePiece(UserId, (byte)place[0], (byte)place[1],
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

    //public async Task Rematch(string gameId)
    //{
    //    ThrowIfPlayerIsNotInGame(gameId);

    //    var rematchResult = ToggleRematch(gameId, UserId);

    //    if (rematchResult.HasValue)
    //    {
    //        await Clients.Group(gameId).SendAsync("Rematch");

    //        await Task.Delay(500);

    //        HandleStartMatch(gameId, rematchResult.Value.player1, rematchResult.Value.player2);
    //    }
    //}

    private void ThrowIfPlayerIsNotInGame(string gameId)
    {
        if (!Games[gameId].Players.ContainsKey(UserId))
            throw new Exception("Player not in group");
    }

    private async void HandleStartMatch(string gameId, KeyValuePair<Guid, Color> player1,
        KeyValuePair<Guid, Color> player2)
    {
        await Clients.Client(player1.Key.ToString()).SendAsync("StartGame", gameId, player1.Value);
        await Clients.Client(player2.Key.ToString()).SendAsync("StartGame", gameId, player2.Value);
        await Clients.AllExcept(player1.ToString(), player2.Key.ToString()).SendAsync("LobbyStarted", gameId);
    }

    private async void EndMatch(string gameId)
    {
        GameService.EndMatch(gameId);
        await Clients.All.SendAsync("GameFinished", gameId);
    }

    private async void HandleLeave(string gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);

        await Clients.All.SendAsync("PlayerLeft", gameId, UserId);

        switch (Games[gameId].State)
        {
            case Game.GameState.Waiting:
                Games[gameId].Players.Remove(UserId);
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

        Games[gameId].Players.Remove(UserId);
    }
}