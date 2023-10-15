using System.Diagnostics;
using Godot;
using Godot4CS.ProjectMuseum.Scripts;


public partial class ProceduralMineGenerator : TileMap
{
	private FastNoiseLite _perlinNoise = new();  
	private Vector2 _initialBlockPosition;
	private Cell[,] _grid;

	[Export] private TileMap _tileMap;
	[Export] private int _totalTiles;

	[Export] private int _cellSize = 64;
	[Export] private int _width = 25;
	[Export] private int _length = 32; //must be 64
	
	private PackedScene _cellPrefab;
	[Export] private string _cellPath;

	[Export] private PlayerController _playerController;

	private RandomNumberGenerator _randomNumberGenerator = new();

	[Export] private float _cellBreakThreshold = 0.9f;
	public override void _Ready()
	{
		_playerController.OnPlayerCollision += CheckIfColliderIsTileMap;
		_cellPrefab = (PackedScene)ResourceLoader.Load(_cellPath);
		GenerateGrid();
	}

	private void Print(KinematicCollision2D collision2D)
	{
		GD.Print("Signal called successfully");
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

	private void CheckIfColliderIsTileMap(KinematicCollision2D collision2D)
	{
		//var collider = collision2D.GetCollider();
		if (collision2D.GetCollider() == _tileMap)
		{
			var tilePos = _tileMap.LocalToMap(_playerController.Position);
			tilePos -= (Vector2I) collision2D.GetNormal();
			var tile = _tileMap.GetCellSourceId(0, tilePos);
			if (tile > 0)
			{
				GD.Print("Changing tile color");
				_tileMap.SetCell(0,tilePos,4,new Vector2I(11,1));
			}
		}
	}
}
