using GameTrilha.GameDomain.Entities;
using System.Collections.Concurrent;
using GameTrilha.API.Helpers;
using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.Services;

// TODO: Refactor to use a database and separate classes to another file
public static class GameService
{
    public static readonly ConcurrentDictionary<string, Game> Games = new(Enumerable.Range(1, 20).ToDictionary(x => x.ToString(), x => new Game()));

    public class Game
    {
        public Dictionary<string, Player> Players { get; set; }
        public Board? Board { get; set; }
        public GameState State { get; set; }

        public Game()
        {
            Players = new Dictionary<string, Player>();
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

    public static string[] GetPlayers(string gameId)
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
        Games[gameId].Players = new Dictionary<string, Player>();
    }

    public static (KeyValuePair<string, Color> player1, KeyValuePair<string, Color> player2) StartGame(string gameId, bool moinhoDuplo)
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
}