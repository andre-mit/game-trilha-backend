using GameTrilha.GameDomain.Entities;
using System.Collections.Concurrent;

namespace GameTrilha.API.Services;

public static class GameService
{
    public static readonly ConcurrentDictionary<string, Game> Games = new(Enumerable.Range(1, 20).ToDictionary(x => x.ToString(), x => new Game()));

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
}