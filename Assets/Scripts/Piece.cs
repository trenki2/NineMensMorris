using System.Collections;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public int player;

    private bool dragging;
    private Vector3 originalPosition;
    private bool moving;
    private Board board;

    private void Start()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
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
        if (!moving && player == 1)
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

            if (board.CanDropHere(transform.position, out var dropIndex))
                board.Drop(dropIndex, player, gameObject);
            else
                StartCoroutine(MoveToOriginalPosition());
        }
    }

    private IEnumerator MoveToOriginalPosition()
    {
        moving = true;

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
        moving = false;
    }
}