using System;
using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
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

    private static Vector2 _lastCheckedPosition = new Vector2(1000, 1000);
    private static bool _lastCheckedResult = false;
    public static bool CheckIfNextPositionIsEmpty(this List<MuseumTile> museumTiles, Vector2 nextPosition)
    {
        Vector2I tilePosition = GameManager.TileMap.LocalToMap(nextPosition);
        if (_lastCheckedPosition == tilePosition)
        {
            return _lastCheckedResult;
        }

        _lastCheckedPosition = tilePosition;
        foreach (var museumTile in museumTiles)
        {
            if (museumTile.XPosition == tilePosition.X && museumTile.YPosition == tilePosition.Y)
            {
                if (museumTile.ExhibitId == "string" || museumTile.ExhibitId == "")
                {
                    _lastCheckedResult = true;
                    return true;
                }
                break;
            }
        }

        _lastCheckedResult = false;
        return false;
    }
    private static int ManhattanDistance(Vector2I a, Vector2I b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
}