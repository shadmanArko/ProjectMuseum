using Godot;
using Godot4CS.ProjectMuseum.Scripts;


public partial class ProceduralMineGenerator : TileMap
{
	private FastNoiseLite _perlinNoise = new();  
	private Vector2 _initialBlockPosition;
	private Cell[,] _grid;

	[Export] private TileMap _tileMap;
	[Export] private int _totalTiles;

	//[Export] private float _noiseScale = 0.1f;

	[Export] private int _cellSize = 64;
	[Export] private int _width = 25;
	[Export] private int _length = 32; //must be 64
	
	private PackedScene _cellPrefab;
	[Export] private string _cellPath; 

	private RandomNumberGenerator _randomNumberGenerator = new();

	[Export] private float _cellBreakThreshold = 0.9f;
	public override void _Ready()
	{
		_cellPrefab = (PackedScene)ResourceLoader.Load(_cellPath);
		GenerateGrid();
	}

	private void GenerateGrid()
	{
		_grid = new Cell[_width, _length];

		for (var y = 0; y < _length; y++)
		{
			for (var x = 0; x < _width; x++)
			{
				_grid[x, y] = InstantiateCell(x,y);
				var tilePos = _tileMap.LocalToMap(_grid[x, y].Pos);
				_tileMap.SetCell(0,tilePos,4,new Vector2I(6,1));
			}
		}
	}

	private Cell InstantiateCell(int width, int height)
	{
		var breakStrength = _randomNumberGenerator.RandiRange(1, 3);
		var cell = new Cell(false, false, breakStrength)
		{
			Pos = new Vector2(width * _cellSize, height * _cellSize)
		};

		return cell;
	}
}
