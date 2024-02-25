using Godot;
using System;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;

public partial class TileAtlasCreator : Node
{
    [Export] private string _texturePath;
    [Export] private int _numberOfColumn;
    [Export] private int _numberOfRow;
    [Export] private int _tileHeight;
    [Export] private int _tileWidth;
    public override void _Ready()
    {
        var atlasSource = new TileSetAtlasSource();

        atlasSource.Texture = (Texture2D)ResourceLoader.Load(_texturePath);

        atlasSource.TextureRegionSize = new Vector2I(_tileWidth, _tileHeight);
        for (int y = 0; y < _numberOfRow; y++)
        {
            for (int x = 0; x < _numberOfColumn; x++)
            {
                atlasSource.CreateTile(new Vector2I(x, y));
            }
        }

        GameManager.TileMap.TileSet.AddSource(atlasSource) ;
    }
}