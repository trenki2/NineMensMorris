using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private struct BoardElement
    {
        public int Player { get; set; }
        public GameObject GameObject { get; set; }
        public Vector2 Position { get; set; }
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
}