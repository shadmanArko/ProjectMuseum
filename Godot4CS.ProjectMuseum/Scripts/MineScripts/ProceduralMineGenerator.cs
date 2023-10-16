using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts;

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

	private void GenerateGrid()
	{
		_grid = new Cell[_width, _length];

		for (var y = 0; y < _length; y++)
		{
			for (var x = 0; x < _width; x++)
			{
				_grid[x, y] = InstantiateCell(x,y);
				var tilePos = _tileMap.LocalToMap(_grid[x, y].Pos);
				_tileMap.SetCell(0,tilePos,1,new Vector2I(0,0));
				GD.Print("Generating Cells");
			}
		}
	}

	private Cell InstantiateCell(int width, int height)
	{
		var breakStrength = 3;
		var cell = new Cell(false, false, breakStrength)
		{
			Pos = new Vector2(width * _cellSize, height * _cellSize)
		};

		return cell;
	}

	private void CheckIfColliderIsTileMap(KinematicCollision2D collision2D)
	{
		GD.Print("inside check if collider is tilemap");
		if (collision2D.GetCollider() == _tileMap)
		{
			var tilePos = _tileMap.LocalToMap(_playerController.Position);
			tilePos -= new Vector2I(Mathf.RoundToInt(collision2D.GetNormal().X),
				Mathf.RoundToInt(collision2D.GetNormal().Y));
			var tile = _tileMap.GetCellSourceId(0, tilePos);
			if (tile > 0)
			{
				GD.Print($"Co-ordinates: {tilePos.X} {tilePos.Y}");
				BreakCell(tilePos);
			}
		}
	}

	private void BreakCell(Vector2I tilePos)
	{
		var cell = _grid[tilePos.X, tilePos.Y];
		GD.Print($"cell strength: {cell.BreakStrength}");
		_grid[tilePos.X, tilePos.Y].BreakStrength--;
		if (cell.BreakStrength >= 2)
		{
			_tileMap.SetCell(0,tilePos,1,new Vector2I(1,0));
		}
		else if (cell.BreakStrength >= 1)
		{
			_tileMap.SetCell(0,tilePos,1,new Vector2I(2,0));
		}
		else
		{
			_tileMap.SetCell(0,tilePos,1,new Vector2I(3,0));
		}
		
	}
}