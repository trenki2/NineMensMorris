﻿using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MillAI
{
    private System.Random random = new System.Random();
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    private int timeLimit = 5000;

    public MillAction CalculateAction(Game game)
    {
        var locker = new object();
        var children = game.Children.OrderBy(x => random.Next()).ToArray();
        var bestResult = float.NegativeInfinity;
        var bestAction = children[0].LastAction;

        stopwatch.Start();

        for (int lookAhead = 2; lookAhead <= 4; lookAhead++)
        {
            Parallel.For(0, children.Length, (i) =>
            {
                var result = AlphaBeta(children[i], lookAhead, float.NegativeInfinity, float.PositiveInfinity, game.Player);
                lock (locker)
                {
                    if (result > bestResult && stopwatch.ElapsedMilliseconds < timeLimit)
                    {
                        bestResult = result;
                        bestAction = children[i].LastAction;
                    }
                }
            });
        }

        return bestAction;
    }

    private float AlphaBeta(Game node, int depth, float alpha, float beta, int maximizingPlayer)
    {
        if (depth == 0 || node.IsGameOver || stopwatch.ElapsedMilliseconds >= timeLimit)
            return node.CalculateRating(maximizingPlayer);

        if (node.Player == maximizingPlayer)
        {
            var value = float.NegativeInfinity;
            foreach (var child in node.Children.OrderBy(x => RandomInt()))
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
            foreach (var child in node.Children.OrderBy(x => RandomInt()))
            {
                value = Math.Min(value, AlphaBeta(child, depth - 1, alpha, beta, maximizingPlayer));
                beta = Math.Min(beta, value);
                if (alpha >= beta)
                    break;
            }
            return value;
        }
    }

    private int RandomInt()
    {
        lock (random)
        {
            return random.Next();
        }
    }
}