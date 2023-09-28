using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts;
using Range = Godot.Range;

public partial class ProceduralMineGenerator : TileMap
{
	[Export] private FastNoiseLite perlinNoise = new();  
	[Export] private Vector2 _initialBlockPosition;
	public Cell[,] grid;

	public float scale = 0.1f;
	public int width = 25;
	public int length = 64;

	private RandomNumberGenerator _randomNumberGenerator = new();

	[Export] private float _cellBreakThreshold = 0.9f;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void GeneratePerlinNoise()
	{
		var noiseMap = new float[width, length];
		for (var y = 0; y < length; y++)
		{
			for (var x = 0; x < width; x++)
			{
				var noiseValue = perlinNoise.GetNoise2D(x * scale,y * scale);
				noiseMap[x, y] = noiseValue;
			}
		}
		
		GenerateGridFromPerlinNoise(noiseMap);
	}

	public void GenerateGridFromPerlinNoise(float[,] noiseMap)
	{
		grid = new Cell[width, length];

		for (int y = 0; y < length; y++)
		{
			for (int x = 0; x < width; x++)
			{
				var cell = new Cell();
				float noiseValue = noiseMap[x, y];

				cell.IsBreakable = noiseValue < _cellBreakThreshold;
				cell.BreakStrength = _randomNumberGenerator.RandiRange(1, 3);
				grid[x, y] = cell;
			}
		}
	}
}
