using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public class MillAI : MonoBehaviour
{
    public int player = 2;

    private enum State
    {
        Begin,
        WaitForResult,
        PlacingStone,
        TakingStone,
        MovingStone
    }

    private object locker = new object();
    private State state = State.Begin;
    private Board board;
    private MillGame game;
    private MillAction action;
    private Queue<GameObject> stones = new Queue<GameObject>();

    private void Start()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        game = board.Game;

        foreach (var obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name.Contains("StoneBlack"))
                stones.Enqueue(obj);
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.Begin:
                if (!game.IsGameOver && game.Player == player)
                {
                    CalculateAction();
                    state = State.WaitForResult;
                }
                break;

            case State.WaitForResult:
                if (ResultAvailable)
                {
                    switch (game.State)
                    {
                        case MillState.PlacingStones:
                            StartCoroutine(PlaceStone());
                            break;

                        case MillState.MovingStones:
                            StartCoroutine(MoveStone());
                            break;

                        case MillState.TakingStones:
                            StartCoroutine(TakeStone());
                            break;
                    }
                }
                break;
        }
    }

    private bool ResultAvailable
    {
        get
        {
            lock (locker)
            {
                return action != null;
            }
        }
    }

    private IEnumerator TakeStone()
    {
        state = State.TakingStone;

        var stone = board.Stones[action.Pos0];
        board.Stones[action.Pos0] = null;
        Destroy(stone);
        game.Execute(action);
        Debug.Assert(game.Board[action.Pos0] == 0);

        state = State.Begin;

        yield return null;
    }

    private IEnumerator MoveStone()
    {
        state = State.MovingStone;

        var stone = board.Stones[action.Pos0];

        var startPos = stone.transform.position;
        var targetPos = board.Positions[action.Pos1];

        var startTime = Time.time;
        var duration = 0.5f;

        while (Time.time - startTime < duration)
        {
            var t = (Time.time - startTime) / duration;
            stone.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return new WaitForEndOfFrame();
        }

        stone.transform.position = targetPos;
        board.Stones[action.Pos0] = null;
        board.Stones[action.Pos1] = stone;
        game.Execute(action);
        Debug.Assert(game.Board[action.Pos1] == player);

        state = State.Begin;
    }

    private IEnumerator PlaceStone()
    {
        state = State.PlacingStone;

        var stone = stones.Dequeue();

        var startPos = stone.transform.position;
        var targetPos = board.Positions[action.Pos0];

        var startTime = Time.time;
        var duration = 0.5f;

        while (Time.time - startTime < duration)
        {
            var t = (Time.time - startTime) / duration;
            stone.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return new WaitForEndOfFrame();
        }

        stone.transform.position = targetPos;
        board.Stones[action.Pos0] = stone;
        game.Execute(action);
        Debug.Assert(game.Board[action.Pos0] == player);

        state = State.Begin;
    }

    private void CalculateAction()
    {
        action = null;

        Task.Run(() =>
        {
            var ai = new AI();
            var action = ai.CalculateAction(board.Game);

            lock (locker)
            {
                this.action = action;
            }
        });
    }
}

public class AI
{
    private System.Random random = new System.Random();
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    private int timeLimit = 5000;

    public MillAction CalculateAction(MillGame game)
    {
        var key = new object();
        var children = game.Children.OrderBy(x => random.Next()).ToArray();
        var bestResult = float.NegativeInfinity;
        var bestAction = children[0].LastAction;

        stopwatch.Start();

        Parallel.For(0, children.Length, (i) =>
        {
            var result = AlphaBeta(children[i], 4, float.NegativeInfinity, float.PositiveInfinity, game.Player);
            lock (key)
            {
                if (result > bestResult)
                {
                    bestResult = result;
                    bestAction = children[i].LastAction;
                }
            }
        });

        return bestAction;
    }

    private float AlphaBeta(MillGame node, int depth, float alpha, float beta, int maximizingPlayer)
    {
        var otherPlayer = maximizingPlayer == 1 ? 2 : 1;

        if (depth == 0 || node.IsGameOver || stopwatch.ElapsedMilliseconds >= timeLimit)
            return node.CalculateRating(maximizingPlayer);

        if (node.Player == maximizingPlayer)
        {
            var value = float.NegativeInfinity;
            foreach (var child in node.Children.OrderBy(x => RandomInt()))
            {
                value = Math.Max(value, AlphaBeta(child, depth - 1, alpha, beta, otherPlayer));
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
        lock(random)
        {
            return random.Next();
        }
    }
}