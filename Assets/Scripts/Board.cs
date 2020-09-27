using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject[] Stones = new GameObject[24];
    public Vector3[] Positions = new Vector3[24];

    private void Start()
    {
        var pos0 = GameObject.Find("BoardPos0").transform.position;
        var pos1 = GameObject.Find("BoardPos1").transform.position;
        var pos2 = GameObject.Find("BoardPos2").transform.position;

        Positions[0] = pos0;
        Positions[1] = new Vector3(0, 0, pos0.z);
        Positions[2] = new Vector3(-pos0.x, 0, pos0.z);
        Positions[3] = new Vector3(-pos0.x, 0, 0);
        Positions[4] = new Vector3(-pos0.x, 0, -pos0.z);
        Positions[5] = new Vector3(0, 0, -pos0.z);
        Positions[6] = new Vector3(pos0.x, 0, -pos0.z);
        Positions[7] = new Vector3(pos0.x, 0, 0);

        Positions[8] = pos1;
        Positions[9] = new Vector3(0, 0, pos1.z);
        Positions[10] = new Vector3(-pos1.x, 0, pos1.z);
        Positions[11] = new Vector3(-pos1.x, 0, 0);
        Positions[12] = new Vector3(-pos1.x, 0, -pos1.z);
        Positions[13] = new Vector3(0, 0, -pos1.z);
        Positions[14] = new Vector3(pos1.x, 0, -pos1.z);
        Positions[15] = new Vector3(pos1.x, 0, 0);

        Positions[16] = pos2;
        Positions[17] = new Vector3(0, 0, pos2.z);
        Positions[18] = new Vector3(-pos2.x, 0, pos2.z);
        Positions[19] = new Vector3(-pos2.x, 0, 0);
        Positions[20] = new Vector3(-pos2.x, 0, -pos2.z);
        Positions[21] = new Vector3(0, 0, -pos2.z);
        Positions[22] = new Vector3(pos2.x, 0, -pos2.z);
        Positions[23] = new Vector3(pos2.x, 0, 0);
    }

    public (Vector3 location, int boardPos, float dist) GetNearestPosition(Vector3 pos)
    {
        float bestDist = float.MaxValue;
        int bestPos = 0;

        for (int i = 0; i < Positions.Length; i++)
        {
            var dist = Vector3.Distance(Positions[i], pos);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestPos = i;
            }
        }

        return (Positions[bestPos], bestPos, bestDist);
    }

    public void MoveStone(int from, int to, GameObject stone)
    {
        if (from != -1)
            Stones[from] = null;
        Stones[to] = stone;
    }

    public void RemoveStone(int boardPos)
    {
        var stone = Stones[boardPos];
        Stones[boardPos] = null;
        Destroy(stone);
    }
}