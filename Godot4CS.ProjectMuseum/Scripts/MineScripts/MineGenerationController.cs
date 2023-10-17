using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts;

public partial class MineGenerationController : Node2D
{
	private Cell[,] _grid;

	private double mouseAngle = 0;

	[Export] private MineGenerationView _mineGenerationView;

	[Export] private int _cellSize = 64;
	[Export] private int _width = 25;
	[Export] private int _length = 32; //must be 64

	[Export] private PlayerController _playerController;

	private RandomNumberGenerator _randomNumberGenerator = new();
    
	public override void _Ready()
	{
		//_playerController.OnPlayerCollision += CheckIfColliderIsTileMap;
		_playerController.OnClickAttack += AttackWall;
		GenerateGrid();
	}

	#region Mine Generator

	private void GenerateGrid()
	{
		_grid = new Cell[_width, _length];

		for (var y = 0; y < _length; y++)
		{
			for (var x = 0; x < _width; x++)
			{
				if (x == 0 || x== _width -1)
				{
					if (x == 0 && y == 17)
					{
						_grid[x, y] = BlankCell(x, y);
						continue;
					}

					_grid[x, y] = InstantiateUnbreakableCell(x, y);
					continue;
				}

				if (y == 0 || y == _length -1)
				{
					_grid[x, y] = InstantiateUnbreakableCell(x, y);
					continue;
				}

				_grid[x, y] = InstantiateCell(x, y);
			}
		}
	}

	private Cell BlankCell(int width, int height)
	{
		var cell = new Cell(false, false, 1000)
		{
			Pos = new Vector2(width * _cellSize, height * _cellSize),
			IsInstantiated = false
		};

		return cell;
	}

	private Cell InstantiateUnbreakableCell(int width, int height)
	{
		var cell = new Cell(false, false, 1000)
		{
			Pos = new Vector2(width * _cellSize, height * _cellSize),
			IsInstantiated = true
		};

		_mineGenerationView.SetCell(0,new Vector2I(Mathf.RoundToInt(cell.Pos.X), Mathf.RoundToInt(cell.Pos.Y)),1,new Vector2I(1,0));
		return cell;
	}

	private Cell InstantiateCell(int width, int height)
	{
		var breakStrength = 3;
		var cell = new Cell(true, false, breakStrength)
		{
			Pos = new Vector2(width * _cellSize, height * _cellSize),
			IsInstantiated = true
		};
		_mineGenerationView.SetCell(0,new Vector2I(Mathf.RoundToInt(cell.Pos.X), Mathf.RoundToInt(cell.Pos.Y)),1,new Vector2I(1,0));

		return cell;
	}

	#endregion

	#region Collision Detection

	private void CheckIfColliderIsTileMap(KinematicCollision2D collision2D)
	{
		GD.Print("inside check if collider is tilemap");
		if (collision2D.GetCollider() == _mineGenerationView)
		{
			var tilePos = _mineGenerationView.LocalToMap(_playerController.Position);
			tilePos -= new Vector2I(Mathf.RoundToInt(collision2D.GetNormal().X),
				Mathf.RoundToInt(collision2D.GetNormal().Y));
			var tile = _mineGenerationView.GetCellSourceId(0, tilePos);
			if (tile > 0)
			{
				GD.Print($"Co-ordinates: {tilePos.X} {tilePos.Y}");
				
				BreakCell(tilePos);
			}
		}
	}

	private void AttackWall()
	{
		var tilePos = _mineGenerationView.LocalToMap(_playerController.Position);
		var mousePos = GetGlobalMousePosition().Normalized().Ceil();
		tilePos += (Vector2I) mousePos;
		GD.Print($"MousePos {mousePos}, Breaking Cell{tilePos}");
		BreakCell(tilePos);
	}
    

	private void BreakCell(Vector2I tilePos)
	{
		if (tilePos.X < 0 || tilePos.Y < 0)
		{
			GD.Print("Wrong cell index");
			return;
		}
		var cell = _grid[tilePos.X, tilePos.Y];
		if (!cell.IsBreakable)
		{
			GD.Print("Is not breakable");
			return;
		}
		GD.Print($"cell strength: {cell.BreakStrength}");
		_grid[tilePos.X, tilePos.Y].BreakStrength--;
		if (cell.BreakStrength >= 2)
		{
			_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(1,0));
		}
		else if (cell.BreakStrength >= 1)
		{
			_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(2,0));
		}
		else
		{
			_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(3,0));
		}
	}

	

	

	#endregion
}