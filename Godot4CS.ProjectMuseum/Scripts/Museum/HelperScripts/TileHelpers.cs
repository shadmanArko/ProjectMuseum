using System;
using System.Collections.Generic;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;

public static class TileHelpers
{
    public static Vector2I GetClosestEmptyTileToExhibit(this List<MuseumTile> museumTiles, Exhibit exhibit)
    {
        Vector2I closestCoordinate = new Vector2I(museumTiles[0].XPosition, museumTiles[0].YPosition);
        Vector2I targetCoordinate = new Vector2I(exhibit.XPosition, exhibit.YPosition);
        float minDistance = float.MaxValue;
    
        foreach (var tile in museumTiles)
        {
            // Skip non-empty or non-walkable tiles
            if ( tile.ExhibitId != "string" && tile.ExhibitId !="")
                continue;
    
            Vector2I currentCoordinate = new Vector2I(tile.XPosition, tile.YPosition);
            int distance = ManhattanDistance(targetCoordinate, currentCoordinate);
    
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCoordinate = currentCoordinate;
            }
        }
    
        return closestCoordinate;
    }
    private static int ManhattanDistance(Vector2I a, Vector2I b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
}