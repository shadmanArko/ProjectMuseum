using Godot;
using System;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;

public partial class TileAtlasCreator : Node
{
    public override void _Ready()
    {
        var atlasSource = new TileSetAtlasSource();

        atlasSource.Texture = (Texture2D)ResourceLoader.Load("res://Assets/2D/Sprites/Floorings/Ceramic 4.png");

        atlasSource.TextureRegionSize = new Vector2I(32, 16);

        atlasSource.CreateTile(new Vector2I(0, 0));

        GameManager.TileMap.TileSet.AddSource(atlasSource) ;
    }
}