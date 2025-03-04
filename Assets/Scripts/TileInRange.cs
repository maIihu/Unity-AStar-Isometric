using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileInRange
{
    public List<TileNode> GetTilesInRange(TileNode currentTile, int range, List<TileNode> availableTiles)
    {
        List<TileNode> tilesInRange = new List<TileNode>();
        int stepCount = 0;
        tilesInRange.Add(currentTile);
        
        List<TileNode> tilesForPreviousStep = new List<TileNode>();
        tilesForPreviousStep.Add(currentTile);
        while (stepCount < range)
        {
            List<TileNode> surroundingTiles = new List<TileNode>();
            foreach (var tile in tilesForPreviousStep)
            {
                surroundingTiles.AddRange(MapManager.Instance.GetNeighbourTiles(tile, availableTiles));
            }
            tilesInRange.AddRange(surroundingTiles);
            tilesForPreviousStep = surroundingTiles.Distinct().ToList();
            stepCount++;
        }

        return tilesInRange.Distinct().ToList();
    }
}
