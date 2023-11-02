using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine;
using Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts;

public partial class MineGenerationController : Node2D
{
	private MineGenerationView _mineGenerationView;
	
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;
    
	public override void _Ready()
	{
		InitializeDiReferences();
		SubscribeToActions();
		_mineGenerationView = GetNode<MineGenerationView>("Mine");
		_mineGenerationVariables.MineGenView = _mineGenerationView;
	}

	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnPlayerAttackAction += AttackWall;
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
		_mineGenerationVariables.Grid = new Cell[_mineGenerationVariables.GridWidth, _mineGenerationVariables.GridLength];

		for (var x = 0; x < _mineGenerationVariables.GridWidth; x++)
		{
			for (var y = 0; y < _mineGenerationVariables.GridLength; y++)
			{
				if (y == 0 || y == _mineGenerationVariables.GridLength -1)
				{
					if (y == 0 && x == 17)
					{
						_mineGenerationVariables.Grid[x, y] = BlankCell(x, y);
						continue;
					}
					
					_mineGenerationVariables.Grid[x, y] = InstantiateUnbreakableCell(x, y);
					continue;
				}

				if (x == 0 || x == _mineGenerationVariables.GridWidth -1)
				{
					_mineGenerationVariables.Grid[x, y] = InstantiateUnbreakableCell(x, y);
					continue;
				}

				_mineGenerationVariables.Grid[x, y] = InstantiateCell(x, y);
			}
		}
	}

	private Cell BlankCell(int width, int height)
	{
		var cell = new Cell(false, false, 1000)
		{
			Pos = new Vector2(width * _mineGenerationVariables.CellSize, height * _mineGenerationVariables.CellSize),
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
			Pos = new Vector2(width * _mineGenerationVariables.CellSize, height * _mineGenerationVariables.CellSize),
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
			Pos = new Vector2(width * _mineGenerationVariables.CellSize, height * _mineGenerationVariables.CellSize),
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
		var tilePos = _mineGenerationView.LocalToMap(_playerControllerVariables.Position);
		var playerPos = _playerControllerVariables.Position;
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
		var cell = _mineGenerationVariables.Grid[tilePos.X, tilePos.Y];
		if (!cell.IsBreakable)
		{
			GD.Print("Is not breakable");
			return;
		}
		GD.Print($"cell strength: {cell.BreakStrength}");
		_mineGenerationVariables.Grid[tilePos.X, tilePos.Y].BreakStrength--;
		Math.Clamp(-_mineGenerationVariables.Grid[tilePos.X, tilePos.Y].BreakStrength, 0, 100);
		
		if (cell.BreakStrength >= 2)
			_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(1,0));
		else if (cell.BreakStrength >= 1)
			_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(2,0));
		else
			_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(4,0));
	}
    
	#endregion
}