using System.Collections;
using UnityEngine;

public class Stone : MonoBehaviour
{
    public int player;

    private bool dragging;
    private Vector3 originalPosition;
    private bool acceptInput = true;
    private Board board;
    private MillGame game;
    private int boardPos = -1;

    private void Start()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        game = board.Game;
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
        if (!acceptInput)
            return;

        if (game.Player != 1)
            return;

        if (game.State == MillState.TakingStones && player == 2)
        {
            var result = board.GetNearestPosition(transform.position);
            var action = new MillAction(result.boardPos);

            if (game.CanExecute(action))
            {
                var stone = board.Stones[result.boardPos];
                board.Stones[result.boardPos] = null;
                Destroy(stone);
                game.Execute(action);
            }
        }
        else if (player == 1 && game.State != MillState.TakingStones)
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

            var result = board.GetNearestPosition(transform.position);
            var action = game.State != MillState.MovingStones ? new MillAction(result.boardPos) : new MillAction(this.boardPos, result.boardPos);
            var canDrop = result.dist < 3.0f && game.CanExecute(action);

            if (canDrop)
                DropStone(result.pos, action);
            else
                StartCoroutine(MoveToOriginalPosition());
        }
    }

    private void DropStone(Vector3 pos, MillAction action)
    {
        if (action.IsMoveAction)
        {
            board.Stones[action.Pos0] = null;
            board.Stones[action.Pos1] = gameObject;
            this.boardPos = action.Pos1;
        }
        else
        {
            board.Stones[action.Pos0] = gameObject;
            this.boardPos = action.Pos0;
        }

        transform.position = pos;
        game.Execute(action);
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
            yield return new WaitForEndOfFrame();
        }

        transform.position = originalPosition;
        acceptInput = true;
    }
}