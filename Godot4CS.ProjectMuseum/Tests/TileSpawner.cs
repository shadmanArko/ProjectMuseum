using Godot;
using System;

public partial class TileSpawner : TileMap
{
	private int ZERO = 0;

	[Export] public int numberOfTilesInX = 32;
	[Export] public int numberOfTilesInY = 20;
	[Export] public int originStartsX = 55;
	[Export] public int originStartsY = 22;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        for (int x = originStartsX; x > originStartsX - numberOfTilesInX; x--)
        {
            for (int y = originStartsY; y > originStartsY - numberOfTilesInY; y--)
            {
                SetCell(0, new Vector2I( x, y), GD.RandRange(0, 2), Vector2I.Zero);
				GD.Print($"{x}, {y}"); 
            }
        }
    }
}
