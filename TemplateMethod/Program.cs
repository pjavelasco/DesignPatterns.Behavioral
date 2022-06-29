using System;
using static System.Console;

namespace Behavioral.TemplateMethod
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var chess = new Chess();
            chess.Run();
        }
    }

    public abstract class Game
    {
        public void Run()
        {
            Start();
            while (!HaveWinner)
                TakeTurn();
            WriteLine($"Player {WinningPlayer} wins.");
        }

        protected abstract void Start();
        protected abstract bool HaveWinner { get; }
        protected abstract void TakeTurn();
        protected abstract int WinningPlayer { get; }

        protected int currentPlayer;
        protected readonly int _numberOfPlayers;

        protected Game(int numberOfPlayers) => _numberOfPlayers = numberOfPlayers;
    }

    // simulate a game of chess
    public class Chess : Game
    {
        public Chess() : base(2)
        {
        }

        protected override void Start()
        {
            WriteLine($"Starting a game of chess with {_numberOfPlayers} players.");
        }

        protected override bool HaveWinner => _turn == _maxTurns;

        protected override void TakeTurn()
        {
            WriteLine($"Turn {_turn++} taken by player {currentPlayer}.");
            currentPlayer = (currentPlayer + 1) % _numberOfPlayers;
        }

        protected override int WinningPlayer => currentPlayer;

        private readonly int _maxTurns = 10;
        private int _turn = 1;
    }
}
