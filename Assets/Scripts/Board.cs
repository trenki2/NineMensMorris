using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour
{
    private class BoardElement
    {
        public int Player { get; set; }
        public GameObject GameObject { get; set; }
        public Vector3 Position { get; set; }
    }

    private List<BoardElement> elements = new List<BoardElement>();

    private void Start()
    {
        var pos0 = GameObject.Find("BoardPos0").transform.position;
        var pos1 = GameObject.Find("BoardPos1").transform.position;
        var pos2 = GameObject.Find("BoardPos2").transform.position;

        elements.Add(new BoardElement { Position = pos0 });
        elements.Add(new BoardElement { Position = new Vector3(0, 0, pos0.z) });
        elements.Add(new BoardElement { Position = new Vector3(-pos0.x, 0, pos0.z) });
        elements.Add(new BoardElement { Position = new Vector3(-pos0.x, 0, 0) });
        elements.Add(new BoardElement { Position = new Vector3(-pos0.x, 0, -pos0.z) });
        elements.Add(new BoardElement { Position = new Vector3(0, 0, -pos0.z) });
        elements.Add(new BoardElement { Position = new Vector3(pos0.x, 0, pos0.z) });
        elements.Add(new BoardElement { Position = new Vector3(pos0.x, 0, 0) });

        elements.Add(new BoardElement { Position = pos1 });
        elements.Add(new BoardElement { Position = new Vector3(0, 0, pos1.z) });
        elements.Add(new BoardElement { Position = new Vector3(-pos1.x, 0, pos1.z) });
        elements.Add(new BoardElement { Position = new Vector3(-pos1.x, 0, 0) });
        elements.Add(new BoardElement { Position = new Vector3(-pos1.x, 0, -pos1.z) });
        elements.Add(new BoardElement { Position = new Vector3(0, 0, -pos1.z) });
        elements.Add(new BoardElement { Position = new Vector3(pos1.x, 0, pos1.z) });
        elements.Add(new BoardElement { Position = new Vector3(pos1.x, 0, 0) });

        elements.Add(new BoardElement { Position = pos2 });
        elements.Add(new BoardElement { Position = new Vector3(0, 0, pos2.z) });
        elements.Add(new BoardElement { Position = new Vector3(-pos2.x, 0, pos2.z) });
        elements.Add(new BoardElement { Position = new Vector3(-pos2.x, 0, 0) });
        elements.Add(new BoardElement { Position = new Vector3(-pos2.x, 0, -pos2.z) });
        elements.Add(new BoardElement { Position = new Vector3(0, 0, -pos2.z) });
        elements.Add(new BoardElement { Position = new Vector3(pos2.x, 0, pos2.z) });
        elements.Add(new BoardElement { Position = new Vector3(pos2.x, 0, 0) });
    }

    public void Drop(int dropIndex, int player, GameObject piece)
    {
        if (dropIndex < 0 || dropIndex > elements.Count || elements[dropIndex].Player != 0)
            throw new ArgumentException("Invalid dropIndex");

        if (player < 1 || player > 2)
            throw new ArgumentException("Invalid player");

        if (piece == null)
            throw new ArgumentNullException("GameObject is null");

        var pos = elements[dropIndex].Position;
        piece.transform.position = new Vector3(pos.x, 0.2f, pos.z);

        elements[dropIndex].Player = player;
        elements[dropIndex].GameObject = piece;
    }

    public bool CanDropHere(Vector3 position, out int dropIndex)
    {
        float bestDist = float.MaxValue;
        int bestIndex = -1;

        for (int i = 0; i < elements.Count; i++)
        {
            var dist = Vector3.Distance(elements[i].Position, position);
            if (dist < bestDist && elements[i].Player == 0)
            {
                bestDist = dist;
                bestIndex = i;
            }
        }

        if (bestIndex != -1 && Vector3.Distance(elements[bestIndex].Position, position) < 2.5f)
        {
            dropIndex = bestIndex;
            return true;
        }

        dropIndex = -1;
        return false;
    }
}