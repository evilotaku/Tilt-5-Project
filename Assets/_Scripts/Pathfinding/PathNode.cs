using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public Vector3Int GridCoords;
    public Vector3 WorldCoords;
    public Grid NavGrid;
    public List<PathNode> Neighbors;
    public PathNode Connection { get; private set; }
    public float G { get; private set; }
    public float H { get; private set; }
    public float F => G + H;
    public bool Walkable = true;

    static Vector3Int
    LEFT = new Vector3Int(-1, 0),
    RIGHT = new Vector3Int(1, 0),
    DOWNRIGHT = new Vector3Int(0, -1),
    DOWNLEFT = new Vector3Int(-1, -1),
    UPRIGHT = new Vector3Int(0, 1),
    UPLEFT = new Vector3Int(-1, 1),
    UPRIGHTODD = new Vector3Int(1, 1),
    UPLEFTODD = new Vector3Int(0, 1),
    DOWNRIGHTODD = new Vector3Int(1, -1),
    DOWNLEFTODD = new Vector3Int(0, -1);


    static Vector3Int[] directions_when_y_is_even =
          { RIGHT, DOWNRIGHT, DOWNLEFT, LEFT, UPLEFT, UPRIGHT };
    static Vector3Int[] directions_when_y_is_odd =
          { RIGHT, DOWNRIGHTODD, DOWNLEFTODD, LEFT, UPLEFTODD, UPRIGHTODD };

    public void SetConnection(PathNode node) => Connection = node;

    public void SetG(float g) => G = g;

    public void SetH(float h) => H = h;

    public void CacheNeighbors()
    {
        Vector3Int[] directions = (GridCoords.z % 2) == 0 ?
              directions_when_y_is_even :
              directions_when_y_is_odd;
        foreach (var direction in directions)
        {
            Vector3Int neighborPos = GridCoords + direction;
            if (WorldGrid.PathMap.TryGetValue(neighborPos, out var pathNode))
                Neighbors.Add(pathNode);
        }
    }

    public float GetDistance(PathNode other)
    {
        return Vector3.Distance(GridCoords, other.GridCoords);
    }
}
