using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Test;

public partial class SpriteSheetToTileMapTest : Node
{
    [Export] private TileMap _tileMap;

    [Export] private int wallCountX;
    [Export] private int wallCountY;

    public override void _Ready()
    {
        CreateWallLayer();
        SetBackDrop();
    }

    private void SetBackDrop()
    {
        _tileMap.SetCell(1,new Vector2I(12,12),0,new Vector2I(0,0));
    }

    private void CreateWallLayer()
    {
        for (var i = 0; i < wallCountX; i++)
        {
            for (var j = 0; j < wallCountY; j++)
            {
                _tileMap.SetCell(0,new Vector2I(i,j),1,new Vector2I(6,0));
            }
        }
    }
}