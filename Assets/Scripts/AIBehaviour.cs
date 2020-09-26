using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
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
    private Game game;
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
            var ai = new MillAI();
            var action = ai.CalculateAction(board.Game);

            lock (locker)
            {
                this.action = action;
            }
        });
    }
}