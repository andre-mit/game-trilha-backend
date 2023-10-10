using GameTrilha.GameDomain.Entities;
using System.Collections.Concurrent;

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

        public Player(bool ready)
        {
            Ready = ready;
            Loaded = false;
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
}