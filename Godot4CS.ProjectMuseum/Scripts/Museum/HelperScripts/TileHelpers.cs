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

        foreach (var tile in museumTiles.Shuffle())
        {
            // Skip non-empty or non-walkable tiles
            if ( !tile.Walkable)
                continue;

            if (tile.XPosition == targetCoordinate.X || tile.YPosition == targetCoordinate.Y)
            {
                Vector2I currentCoordinate = new Vector2I(tile.XPosition, tile.YPosition);
                int distance = ManhattanDistance(targetCoordinate, currentCoordinate);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCoordinate = currentCoordinate;
                }
            }
        
        
        }
    
        return closestCoordinate;
    }
    public static TargetWthOrigin GetRandomEmptyTileClosestToExhibit(this List<MuseumTile> museumTiles, Exhibit exhibit)
    {
        List<TargetWthOrigin> closestCoordinates = new List<TargetWthOrigin>();
        
        foreach (var tileId in exhibit.OccupiedTileIds)
        {
            var tile = museumTiles.GetTileById(tileId);
            Vector2I coord = new Vector2I(tile.XPosition + 1, tile.YPosition);
            if (museumTiles.IsTileWalkable(coord)) closestCoordinates.Add(new TargetWthOrigin(coord, new Vector2I(tile.XPosition, tile.YPosition)));
            coord = new Vector2I(tile.XPosition - 1, tile.YPosition);
            if (museumTiles.IsTileWalkable(coord)) closestCoordinates.Add(new TargetWthOrigin(coord, new Vector2I(tile.XPosition, tile.YPosition)));
            coord = new Vector2I(tile.XPosition , tile.YPosition+1);
            if (museumTiles.IsTileWalkable(coord)) closestCoordinates.Add(new TargetWthOrigin(coord, new Vector2I(tile.XPosition, tile.YPosition)));
            coord = new Vector2I(tile.XPosition , tile.YPosition-1);
            if (museumTiles.IsTileWalkable(coord)) closestCoordinates.Add(new TargetWthOrigin(coord, new Vector2I(tile.XPosition, tile.YPosition)));
            
        }
    
        return closestCoordinates.Shuffle()[0];
    }
    public static Vector2I GetClosestEmptyTileToCoordinate(this List<MuseumTile> museumTiles, Vector2I targetCoordinate)
    {
        Vector2I closestCoordinate = new Vector2I(museumTiles[0].XPosition, museumTiles[0].YPosition);
        float minDistance = float.MaxValue;
    
        foreach (var tile in museumTiles)
        {
            // Skip non-empty or non-walkable tiles
            if ( !tile.Walkable)
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
    public static DecorationShop GetClosestShopToLocation(this List<DecorationShop> shops, Vector2I currentPosition)
    {
        DecorationShop closestShop = shops[0];
        float minDistance = 99999;
        foreach (var shop in shops)
        {
            Vector2I currentShopCoord = new Vector2I(shop.XPosition, shop.YPosition);
            int distance = ManhattanDistance(currentPosition, currentShopCoord);
            GD.Print($"distance of {shop.ShopVariationName} {distance}, min dis {minDistance}");
            if (distance < minDistance)
            {
                minDistance = distance;
                closestShop = shop;
            }
        }
        
        return closestShop;
    }
    public class TargetWthOrigin
    {
        public Vector2I target;
        public Vector2I origin;

        public TargetWthOrigin(Vector2I target, Vector2I origin)
        {
            this.target = target;
            this.origin = origin;
        }
    }
    public static Sanitation GetClosestWashroomToLocation(this List<Sanitation> sanitations, Vector2I currentPosition)
    {
        Sanitation closestWashroom = sanitations[0];
        float minDistance = float.MaxValue;
        foreach (var shop in sanitations)
        {
            Vector2I currentClosestCoordinate = new Vector2I(closestWashroom.XPosition, closestWashroom.YPosition);
            int distance = ManhattanDistance(currentPosition, currentClosestCoordinate);
    
            if (distance < minDistance)
            {
                minDistance = distance;
                closestWashroom = shop;
            }
        }

        return closestWashroom;
    }
    public static Vector2I GetClosestEmptyTileToClosestShop(this List<MuseumTile> museumTiles, Exhibit exhibit)
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
        Vector2I tilePosition = GameManager.tileMap.LocalToMap(nextPosition);
        if (_lastCheckedPosition == tilePosition)
        {
            return _lastCheckedResult;
        }

        _lastCheckedPosition = tilePosition;
        foreach (var museumTile in museumTiles)
        {
            if (museumTile.XPosition == tilePosition.X && museumTile.YPosition == tilePosition.Y)
            {
                if ((museumTile.ExhibitId == "string" || museumTile.ExhibitId == "") && museumTile.Walkable)
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
    public static MuseumTile GetTileById(this List<MuseumTile> museumTiles, string tileId)
    {
        foreach (var museumTile in museumTiles)
        {
            if (museumTile.Id == tileId)
            {
                return museumTile;
            }
        }

        return null;
    }
    public static bool IsTileWalkable(this List<MuseumTile> museumTiles, Vector2I tileCoord)
    {
        foreach (var museumTile in museumTiles)
        {
            if (museumTile.XPosition == tileCoord.X && museumTile.YPosition == tileCoord.Y)
            {
                return museumTile.Walkable;
            }
        }

        return false;
    }
    
    private static int ManhattanDistance(Vector2I a, Vector2I b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
}