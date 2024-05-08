using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;

public static class TilemapHelpers
{
    public static bool IsWorldPositionInsideTileMap(this Vector2 position, TileMap tileMap)
    {
        // Assuming _tileMap is a reference to your Isometric TileMap node
        Vector2 tilemapPos = tileMap.LocalToMap(position);
    
        // Assuming tileSize is the size of your isometric tile
        Vector2 tileSize = GetTileSize(); // Replace with your actual tile size
    
        return tileSize.Y < tilemapPos.Y && tilemapPos.Y <= 0 &&
               tileSize.X < tilemapPos.X && tilemapPos.X <= 0;
    }
    public static bool IsTilePositionInsideTileMap(this Vector2I position, List<MuseumTile> museumTiles)
    {
        
        // Vector2 tileSize = GetTileSize(); // Replace with your actual tile size
        //
        // return tileSize.Y < position.Y && position.Y <= 0 &&
        //        tileSize.X < position.X && position.X <= 0;
        return museumTiles.FirstOrDefault(tile => tile.XPosition == position.X && tile.YPosition == position.Y) != null;
    }

    private static Vector2 GetTileSize()
    {
        return new Vector2(-18, -20);
    }
}