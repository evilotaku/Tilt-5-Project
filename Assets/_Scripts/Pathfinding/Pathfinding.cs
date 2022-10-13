using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting.FullSerializer;

public static class Pathfinding  
{

    public static List<PathNode> FindPath(PathNode start, PathNode target)
    {
        Debug.Log($"Starting Pathfinding from Hex {start.WorldCoords} to Hex {target.WorldCoords}...");
        var toSearch = new List<PathNode>() { start };
        var processed = new List<PathNode>();

        while(toSearch.Any())
        {
            
            var current = toSearch[0];
            foreach (var node in toSearch)
            {
                if (node.F < current.F || node.F == current.F && node.H < current.H) current = node;
            }

            processed.Add(current);
            toSearch.Remove(current);

            if (current == target)
            {
                Debug.Log("Found Path!");
                var currentPathTile = target;
                var path = new List<PathNode>();
                
                while (currentPathTile != start)
                {
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.Connection;                               
                }
               
                Debug.Log(path.Count);
                for (int i = 0; i < path.Count - 1 ; i++)
                {
                    Debug.DrawLine(path[i].WorldCoords, path[i + 1].WorldCoords, Color.red);
                }
                return path;
            }

            if (current.Neighbors.Count == 0) Debug.Log($"No Neighbors found for Hex {current.GridCoords}");
            foreach (var neighbor in current.Neighbors.Where(t => t.Walkable && !processed.Contains(t)))
            {
                Debug.Log($"Evaluating Hex {current.WorldCoords}");
                var inSearch = toSearch.Contains(neighbor);

                var costToNeighbor = current.G + current.GetDistance(neighbor);

                if (!inSearch || costToNeighbor < neighbor.G)
                {
                    neighbor.SetG(costToNeighbor);
                    neighbor.SetConnection(current);

                    if (!inSearch)
                    {
                        neighbor.SetH(neighbor.GetDistance(target));
                        toSearch.Add(neighbor);                       
                    }
                }
            }
        }
        return null;
    }
}

