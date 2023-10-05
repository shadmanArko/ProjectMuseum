using Godot;
using System;

public partial class TileSpawner : TileMap
{
	private int ZERO = 0;

	[Export] public int numberOfTilesInX = 32;
	[Export] public int numberOfTilesInY = 20;
	[Export] public int originStartsY = -50;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        for (int x = 0; x < numberOfTilesInX; x++)
        {
            for (int y = originStartsY; y > originStartsY - numberOfTilesInY; y--)
            {
                SetCell(ZERO, new Vector2I( x, y), ZERO, Vector2I.Zero);
				GD.Print($"{x}, {y}");
            }
        }
    }
}
