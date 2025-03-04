using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{
    public List<TileNode> FindPath(TileNode start, TileNode end, List<TileNode> availableTiles)
    {
        var mapManager = MapManager.Instance;

        List<TileNode> openList = new List<TileNode>();
        List<TileNode> closedList = new List<TileNode>();

        openList.Add(start);

        while (openList.Count > 0)
        {
            TileNode currentOverlayTile = openList.OrderBy(x => x.F).First();
            openList.Remove(currentOverlayTile);
            closedList.Add(currentOverlayTile);

            if (currentOverlayTile == end)
            {
                List<TileNode> result = GetFinishedList(start, end);
                return result;
            }

            foreach (var tile in mapManager.GetNeighbourTiles(currentOverlayTile, availableTiles))
            {
                if (closedList.Contains(tile) || openList.Contains(tile))
                {
                    continue;
                }

                tile.G = GetManhattanDistance(start, tile);
                tile.H = GetManhattanDistance(end, tile);
                tile.previousNode = currentOverlayTile;

                openList.Add(tile);
                
            }
        }

        return new List<TileNode>();
    }
    
    private List<TileNode> GetFinishedList(TileNode start, TileNode end)
    {
        List<TileNode> finishedList = new List<TileNode>();
        TileNode currentTile = end;

        while (currentTile != start)
        {
            finishedList.Add(currentTile);
            currentTile = currentTile.previousNode;
        }

        finishedList.Reverse();

        return finishedList;
    }

    private int GetManhattanDistance(TileNode start, TileNode tile)
    {
        return Mathf.Abs(start.gridLocationInt.x - tile.gridLocationInt.x) + Mathf.Abs(start.gridLocationInt.y - tile.gridLocationInt.y);
    }




}