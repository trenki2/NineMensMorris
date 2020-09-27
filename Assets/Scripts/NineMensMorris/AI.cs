using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NineMensMorris
{
    public class AI
    {
        private Random random = new Random();
        private Stopwatch stopwatch = new Stopwatch();
        private int timeLimit = 5000;

        public Move CalculateMove(Game game)
        {
            var locker = new object();

            var nextStates = game.NextGameStates.OrderBy(x => random.Next()).ToArray();
            if (nextStates.Length == 0)
                return null;

            var bestResult = float.NegativeInfinity;
            var bestMove = nextStates[0].LastMove;

            stopwatch.Start();

            for (int lookAhead = 3; lookAhead <= 6; lookAhead++)
            {
                Parallel.For(0, nextStates.Length, (i) =>
                {
                    var result = AlphaBeta(nextStates[i], lookAhead, float.NegativeInfinity, float.PositiveInfinity, game.CurrentPlayer);

                    lock (locker)
                    {
                        if (result >= bestResult && stopwatch.ElapsedMilliseconds < timeLimit)
                        {
                            bestResult = result;
                            bestMove = nextStates[i].LastMove;
                        }
                    }
                });
            }

            return bestMove;
        }

        private float AlphaBeta(Game game, int depth, float alpha, float beta, int maximizingPlayer)
        {
            if (depth == 0 || game.IsGameOver || stopwatch.ElapsedMilliseconds >= timeLimit)
                return CalculateRating(game, maximizingPlayer);

            if (game.CurrentPlayer == maximizingPlayer)
            {
                var value = float.NegativeInfinity;
                foreach (var child in game.NextGameStates.OrderBy(x => RandomInt()))
                {
                    value = Math.Max(value, AlphaBeta(child, depth - 1, alpha, beta, maximizingPlayer));
                    alpha = Math.Max(alpha, value);
                    if (alpha >= beta)
                        break;
                }
                return value;
            }
            else
            {
                var value = float.PositiveInfinity;
                foreach (var child in game.NextGameStates.OrderBy(x => RandomInt()))
                {
                    value = Math.Min(value, AlphaBeta(child, depth - 1, alpha, beta, maximizingPlayer));
                    beta = Math.Min(beta, value);
                    if (alpha >= beta)
                        break;
                }
                return value;
            }
        }

        private float CalculateRating(Game game, int player)
        {
            var otherPlayer = player == 2 ? 1 : 2;
            var playerStones = game.AvailableStones[player - 1] + game.StonesOnBoard[player - 1];
            var otherPlayerStones = game.AvailableStones[otherPlayer - 1] + game.StonesOnBoard[otherPlayer - 1];
            return playerStones - otherPlayerStones;
        }

        private int RandomInt()
        {
            lock (random)
            {
                return random.Next();
            }
        }
    }
}

