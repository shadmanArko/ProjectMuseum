using System;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts;

public partial class MineGenerationController : Node2D
{
	[Export] public MineGenerationView _mineGenerationView;
	[Export] private PlayerController _playerController;

	private Cell[,] _grid;
	[Export] private int _cellSize = 16;
	[Export] private int _width = 35;
	[Export] private int _length = 64;
    
	public override void _Ready()
	{
		
		//InitializeDiReferences();

		// MineActions.OnPlayerAttackAction += AttackWall;
		// GenerateGrid();
		// var pos = _grid[_width / 2, 0].Pos + new Vector2(0,-15);
		// _playerController.Position = pos;
	}

	private void InitializeDiReferences()
	{
		_playerController = ServiceRegistry.Resolve<PlayerController>();
		_mineGenerationView = GetNode<MineGenerationView>("Mine");
	}

	public void GenerateMine()
	{
		InitializeDiReferences();
		GenerateGrid();
		GD.Print($"mine generation view is null {_mineGenerationView is null}");
	}

	#region Mine Generator

	private void GenerateGrid()
	{
		_grid = new Cell[_width, _length];

		for (var x = 0; x < _width; x++)
		{
			for (var y = 0; y < _length; y++)
			{
				if (y == 0 || y == _length -1)
				{
					if (y == 0 && x == 17)
					{
						_grid[x, y] = BlankCell(x, y);
						continue;
					}
					
					_grid[x, y] = InstantiateUnbreakableCell(x, y);
					continue;
				}

				if (x == 0 || x == _width -1)
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
		var tilePos = _mineGenerationView.LocalToMap(cell.Pos);
		_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(4,0));
		return cell;
	}

	private Cell InstantiateUnbreakableCell(int width, int height)
	{
		var cell = new Cell(false, false, 1000)
		{
			Pos = new Vector2(width * _cellSize, height * _cellSize),
			IsInstantiated = true
		};
		var tilePos = _mineGenerationView.LocalToMap(cell.Pos);
		_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(3,0));
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
		var tilePos = _mineGenerationView.LocalToMap(cell.Pos);
		_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(0,0));

		return cell;
	}

	#endregion

	#region Wall Attack Detection
    
	private void AttackWall()
	{
		var tilePos = _mineGenerationView.LocalToMap(_playerController.Position);
		var playerPos = _playerController.Position;
		var mousePos = GetGlobalMousePosition() - playerPos;
		var angle = GetAngleTo(mousePos);
		var degree = angle * (180 / Math.PI);

		var newPos = degree switch
		{
			<= 45 and > -45 => new Vector2I(1,0),
			<= -45 and > -135 => new Vector2I(0,-1),
			> 45 and <= 135 => new Vector2I(0,1),
			_ => new Vector2I(-1,0)
		};
        
		tilePos += newPos;
		GD.Print($"Breaking Cell{tilePos}");
		GD.Print($"newPos: {newPos}");
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
		Math.Clamp(-_grid[tilePos.X, tilePos.Y].BreakStrength, 0, 100);
		
		if (cell.BreakStrength >= 2)
			_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(1,0));
		else if (cell.BreakStrength >= 1)
			_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(2,0));
		else
			_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(4,0));
	}
    
	#endregion

	#region Setters and Getters

	public Cell[,] GetGrid() => _grid;

	#endregion
}