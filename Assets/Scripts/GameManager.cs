using NineMensMorris;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Game Game { get; } = new Game();
    public MoveState MoveState { get; private set; } = MoveState.MoveStone;

    private Move currentMove = null;

    private void ExecuteCurrentMove()
    {
        Game.Move(currentMove);
        MoveState = MoveState.MoveStone;
        currentMove = null;
    }

    public void RemoveStone(int boardPos)
    {
        currentMove.Remove = boardPos;
        ExecuteCurrentMove();
    }

    public void MoveStone(int from, int to)
    {
        currentMove = new Move
        {
            From = from,
            To = to,
            Remove = -1
        };

        if (Game.WillHaveMill(from, to))
            MoveState = MoveState.RemoveStone;
        else
            ExecuteCurrentMove();
    }
}