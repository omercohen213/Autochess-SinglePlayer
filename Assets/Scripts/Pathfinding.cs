using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using Debug = UnityEngine.Debug;

public class Pathfinding : MonoBehaviour
{
    private List<Hex> toSearch = new();
    private List<Hex> processed = new();
    private readonly int MOVE_COST = 1;

    // Find the shortest path from starting hex to a target hex using A* algorithm
    public List<Hex> FindShortestPath(Hex startHex, Hex targetHex)
    {
        if (startHex == null || targetHex == null)
        {
            Debug.LogWarning("null start or target hex " + gameObject.GetComponent<GameUnit>().UnitName);
            return null;
        }

        startHex.GCost = 0;
        startHex.HCost = 0;
        toSearch.Clear();
        processed.Clear();
        toSearch.Add(startHex);

        while (toSearch.Any())
        {
            // Find the lowest cost hex
            Hex currentHex = toSearch[0];
            foreach (Hex hexToSearch in toSearch)
            {
                if (hexToSearch.FCost < currentHex.FCost || hexToSearch.FCost == currentHex.FCost && hexToSearch.HCost < currentHex.HCost)
                {
                    currentHex = hexToSearch;
                }
            }

            // Mark the chosen hex as processed
            processed.Add(currentHex);
            toSearch.Remove(currentHex);

            if (currentHex == targetHex)
            {
                // Reconstruct the path
                Hex currentPathTile = targetHex;
                List<Hex> path = new();

                while (currentPathTile != startHex)
                {
                    path.Add(currentPathTile);
                    currentPathTile = currentPathTile.ConnectedHex;
                }
                path.Reverse(); // Reverse to get the correct order
                return path;
            }

            foreach (Hex adjacentHex in currentHex.AdjacentHexes.Where(hex => !processed.Contains(hex)))
            {
                bool inSearch = toSearch.Contains(adjacentHex);
                int costToAdjacentHex = currentHex.GCost + MOVE_COST;

                if (!inSearch || costToAdjacentHex < adjacentHex.GCost)
                {
                    bool ignoreIsTaken = adjacentHex == targetHex && adjacentHex.IsTaken;
                    if (!adjacentHex.IsTaken || ignoreIsTaken)
                    {
                        adjacentHex.GCost = costToAdjacentHex;
                        adjacentHex.ConnectedHex = currentHex;

                        if (!inSearch)
                        {
                            adjacentHex.HCost = GetHeuristicDistance(adjacentHex, targetHex);
                            toSearch.Add(adjacentHex);
                        }
                    }
                }
            }
        }
        return null;
    }

    public float GetHeuristicDistance(Hex currentHex, Hex targetHex)
    {
        return MOVE_COST * Vector3.Distance(currentHex.transform.position, targetHex.transform.position);
    }

    // Find closest enemy unit on board
    public Hex FindClosestEnemy(List<GameUnit> enemyUnits, Hex currentHex)
    {
        Hex closestHex = null;
        float shortestDistance = float.MaxValue;

        foreach (GameUnit enemyUnit in enemyUnits)
        {
            // Calculate distance between current unit and enemy unit
            if (currentHex != null && enemyUnit.CurrentHex != null)
            {
                float distance = Vector2Int.Distance(currentHex.ToVector2Int(), enemyUnit.CurrentHex.ToVector2Int());

                // Check if this enemy unit is closer than the previously found closest one
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestHex = enemyUnit.CurrentHex;
                }
            }
        }
        return closestHex;
    }

}
