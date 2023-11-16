using GameTrilha.GameDomain.Entities;
using System.Collections.Concurrent;
using GameTrilha.API.Helpers;
using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.Services;

// TODO: Refactor to use a database and separate classes to another file
public static class GameService
{
    public static readonly ConcurrentDictionary<string, Game> Games = new(Enumerable.Range(1, 20).ToDictionary(x => x.ToString(), x => new Game()));
    
    public static event EventHandler<(string gameId, Color turn)> TurnSkippedInGame;

    public class Game
    {
        public Dictionary<Guid, Player> Players { get; set; }
        public Board? Board { get; set; }
        public GameState State { get; set; }
        public Guid MatchId { get; set; }

        public Game()
        {
            Players = new Dictionary<Guid, Player>();
            State = GameState.Waiting;
            Board = null;
        }

        public enum GameState
        {
            Waiting,
            Playing,
            Finished
        }
    }

    public class Player
    {
        public bool Ready { get; set; }
        public bool Loaded { get; set; }
        public bool Rematch { get; set; }

        public Player(bool ready)
        {
            Ready = ready;
            Loaded = false;
            Rematch = false;
        }
    }

    public static Guid[] GetPlayers(string gameId)
    {
        return Games[gameId].Players.Select(player => player.Key).ToArray();
    }

    public static void EndMatch(string gameId)
    {
        Games[gameId].State = Game.GameState.Finished;
    }

    public static void ResetGame(string gameId)
    {
        Games[gameId].State = Game.GameState.Waiting;
        Games[gameId].Board = null;
        Games[gameId].Players = new Dictionary<Guid, Player>();
    }

    public static (KeyValuePair<Guid, Color> player1, KeyValuePair<Guid, Color> player2) StartGame(string gameId, bool moinhoDuplo, Guid matchId)
    {
        Games[gameId].State = Game.GameState.Playing;
        Games[gameId].MatchId = matchId;

        var player1 =
            new KeyValuePair<Guid, Color>(Games[gameId].Players.ElementAt(0).Key, RandomColor.GetRandomColor());
        var player2 =
            new KeyValuePair<Guid, Color>(Games[gameId].Players.ElementAt(1).Key, RandomColor.GetOppositeColor(player1.Value));

        var players = new Dictionary<string, Color>
        {
            { player1.Key.ToString(), player1.Value },
            { player2.Key.ToString(), player2.Value }
        };


        Games[gameId].Board = new Board(moinhoDuplo, players);

        return (player1, player2);
    }

    ///// <summary>
    ///// Set rematch to player
    ///// </summary>
    ///// <param name="gameId">Game id</param>
    ///// <param name="playerId">Player Id</param>
    ///// <returns>True if all players set rematch and start another game session</returns>
    //public static (KeyValuePair<string, Color> player1, KeyValuePair<string, Color> player2)? ToggleRematch(string gameId, string playerId)
    //{
    //    Games[gameId].Players[playerId].Rematch = !Games[gameId].Players[playerId].Rematch;

    //    var players = Games[gameId].Players;

    //    if (players.Count != 2 || !players.All(x => x.Value.Rematch)) return null;

    //    var moinhoDuplo = Games[gameId].Board!.MoinhoDuplo;
    //    var (player1, player2) = StartGame(gameId, moinhoDuplo);
    //    return (player1, player2);
    //}

    private static Color GetTurnPlayerColor(string gameId)
    {
        return Games.TryGetValue(gameId, out var game) ? game.Board?.Turn ?? Color.White : Color.White;
    }

    private static void OnTurnSkippedEvent(string gameId)
    {
        var turnColor = GetTurnPlayerColor(gameId);

        TurnSkippedInGame?.Invoke(null, (gameId, turnColor));
    }
}