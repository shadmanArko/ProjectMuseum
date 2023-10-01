using Godot;
using Godot4CS.ProjectMuseum.Scripts;


public partial class ProceduralMineGenerator : TileMap
{
	private FastNoiseLite _perlinNoise = new();  
	private Vector2 _initialBlockPosition;
	private Cell[,] _grid;

	[Export] private TileMap _tileMap;
	[Export] private int _totalTiles;

	[Export] private float _noiseScale = 0.1f;

	[Export] private int _cellSize = 64;
	[Export] private int _width = 25;
	[Export] private int _length = 32; //must be 64

	private RandomNumberGenerator _randomNumberGenerator = new();

	[Export] private float _cellBreakThreshold = 0.9f;
	public override void _Ready()
	{
		//_tileMap = GetNode<TileMap>();
		GeneratePerlinNoise();
	}

	private void GeneratePerlinNoise()
	{
		var noiseMap = new float[_width, _length];
		for (var y = 0; y < _length; y++)
		{
			for (var x = 0; x < _width; x++)
			{
				var noiseValue = _perlinNoise.GetNoise2D(x * _noiseScale,y * _noiseScale);
				noiseMap[x, y] = noiseValue;
			}
		}
		
		GenerateGridFromPerlinNoise(noiseMap);
	}

	private void GenerateGridFromPerlinNoise(float[,] noiseMap)
	{
		_grid = new Cell[_width, _length];

		for (var y = 0; y < _length; y++)
		{
			for (var x = 0; x < _width; x++)
			{
				var noiseValue = noiseMap[x, y];

				var isBreakable = noiseValue < _cellBreakThreshold;
				var breakStrength = _randomNumberGenerator.RandiRange(1, 3);

				var cell = new Cell(isBreakable, false, breakStrength)
				{
					Pos = new Vector2(x * _cellSize, y * _cellSize)
				};

				var tileIndex = Mathf.RoundToInt(noiseValue * (_totalTiles - 1));
				//cell.Sprite2D = _tileMap.TileSet.
				_grid[x, y] = cell;
				var tilePos = _tileMap.LocalToMap(_grid[x, y].Pos);
				_tileMap.SetCell(0,tilePos,4,new Vector2I(6,1));
				GD.Print($"Set to tile size");
			}
		}
	}
}
