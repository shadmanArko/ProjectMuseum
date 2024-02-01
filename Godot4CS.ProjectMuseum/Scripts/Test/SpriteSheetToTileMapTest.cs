using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Test;

public partial class SpriteSheetToTileMapTest : Node
{
	[Export] private string _spriteSheetPath;
	[Export] private Sprite2D _sprite2D;

	public override void _Ready()
	{
		TurnImageToTileMap();
	}

	public void TurnImageToTileMap()
	{
// 		// Load an image of any format supported by Godot from the filesystem.
// 		var image = Image.LoadFromFile("path/to/image.png");
//
// Create a new `TileMap` node and add it to the scene.
		var tilemap = new TileMap();
		AddChild(tilemap);

// Create a new `TileSet` resource.
		var tileSet = new TileSet();

// Load the image into the `TileSet` resource.
		// var texture = new ImageTexture();
		//texture.CreateFromImage(image);
		var texture = ResourceLoader.Load<Texture2D>(_spriteSheetPath);
		_sprite2D.Texture = texture;
		ModifyTileSet(tileSet);
		tilemap.TileSet = tileSet;
		
		
		
		//tileSet.CreateTile(0);
		//tileSet.TileSetTexture(0, texture);

// Set the `TileSet` of the `TileMap` node to the loaded `TileSet` resource.
		tilemap.TileSet = tileSet;
	}

	private void ModifyTileSet(TileSet tileSet)
	{
		// tileSet.
		tileSet.TileShape = TileSet.TileShapeEnum.Square;
		tileSet.TileSize = new Vector2I(20, 20);
		// tileSet
	}
	
}