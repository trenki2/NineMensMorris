using System.Collections;
using UnityEngine;
using NineMensMorris;

public class Stone : MonoBehaviour
{
    public int owner;

    private Board board;
    private Game game;
    private GameManager gameManager;

    private bool dragging;
    private bool acceptInput = true;
    
    private int boardPos = -1;
    
    private Vector3 originalPosition;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        board = GameObject.Find("Board").GetComponent<Board>();
        game = gameManager.Game;
    }

    private void Update()
    {
        if (dragging)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var layerMask = LayerMask.GetMask("Board", "Table");
            var offset = 0.2f;
            if (Physics.Raycast(ray, out var hitinfo, float.MaxValue, layerMask))
                transform.position = new Vector3(hitinfo.point.x, hitinfo.point.y + offset, hitinfo.point.z);
        }
    }

    private void OnMouseDown()
    {
        if (!acceptInput || game.CurrentPlayer != 1 || game.IsGameOver)
            return;

        if (owner == 2 && gameManager.MoveState == MoveState.RemoveStone)
        {
            var pos = board.GetNearestPosition(transform.position);
            if (game.CanRemove(pos.boardPos))
            {
                board.RemoveStone(pos.boardPos);
                gameManager.RemoveStone(pos.boardPos);
            }
        }
        else if (owner == 1 && gameManager.MoveState == MoveState.MoveStone)
        {
            dragging = true;
            originalPosition = transform.position;
        }
    }

    private void OnMouseUp()
    {
        if (dragging) 
        {
            dragging = false;

            var pos = board.GetNearestPosition(transform.position);
            var canDrop = pos.dist < 3.0f && game.Board[pos.boardPos] == 0;

            if (canDrop && game.IsValidFromTo(boardPos, pos.boardPos))
            {
                board.MoveStone(from: boardPos, to: pos.boardPos, gameObject);
                gameManager.MoveStone(from: boardPos, to: pos.boardPos);
                boardPos = pos.boardPos;
                transform.position = pos.location;
            }
            else
            {
                StartCoroutine(MoveToOriginalPosition());
            }
        }
    }

    private IEnumerator MoveToOriginalPosition()
    {
        acceptInput = false;

        var startPos = transform.position;
        var startTime = Time.time;
        var duration = 0.5f;

        while (Time.time - startTime < duration)
        {
            var t = (Time.time - startTime) / duration;
            transform.position = Vector3.Lerp(startPos, originalPosition, t);
            yield return null;
        }

        transform.position = originalPosition;
        acceptInput = true;
    }
}