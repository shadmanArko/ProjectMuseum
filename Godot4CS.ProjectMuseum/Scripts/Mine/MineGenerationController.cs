using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.MineScripts;
using Godot4CS.ProjectMuseum.Scripts.MineScripts.PlayerScripts;
using Newtonsoft.Json;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineGenerationController : Node2D
{
	private global::ProjectMuseum.Models.Mine _mine = new global::ProjectMuseum.Models.Mine();
	
	private HttpRequest _saveGeneratedMineHttpRequest;
	private HttpRequest _getGeneratedMineHttpRequest;
	
	private MineGenerationView _mineGenerationView;
	
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;
    
	public override void _Ready()
	{
		_saveGeneratedMineHttpRequest = new HttpRequest();
		AddChild(_saveGeneratedMineHttpRequest);
		_saveGeneratedMineHttpRequest.RequestCompleted += OnSaveGeneratedMineHttpRequestComplete;
		
		_getGeneratedMineHttpRequest = new HttpRequest();
		AddChild(_getGeneratedMineHttpRequest);
		_getGeneratedMineHttpRequest.RequestCompleted += OnGetMineDataRequestCompleted;
		
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
		//GetMineDataFromServer();
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustReleased("generateGrid"))
			GenerateMine();
		
		if(Input.IsActionJustReleased("saveGrid"))
			SaveMineDataIntoServer();
			
		if(Input.IsActionJustReleased("loadGrid"))
			GetMineDataFromServer();
	}

	#region Save Mine Data Into Server

	private void SaveMineDataIntoServer()
	{
		string[] headers = { "Content-Type: application/json"};
		_mine.Cells = Cells2DArrayToList();
		_mine.CellSize = 16;
		_mine.GridLength = 64;
		_mine.GridWidth = 35;
		
		var body = JsonConvert.SerializeObject(_mine);

		Error error = _saveGeneratedMineHttpRequest.Request("http://localhost:5178/api/MuseumTile/UpdateMineData", headers,
			HttpClient.Method.Put, body);
	}
	
	private List<Cell> Cells2DArrayToList()
	{
		var cellList = new List<Cell>();

		for (var y = 0; y < _mineGenerationVariables.GridLength; y++)
		{
			for (var x = 0; x < _mineGenerationVariables.GridWidth; x++)
			{
				cellList.Add(_mineGenerationVariables.Cells[x,y]);
			}
		}

		return cellList;
	}
	
	private void OnSaveGeneratedMineHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print("ON SAVE GENERATED MINE HTTP REQUEST COMPLETE method called");
	}

	#endregion

	#region Get Mine Data From Server

	private void GetMineDataFromServer()
	{
		GD.Print("RETRIEVING CELL LIST");
		var url = "http://localhost:5178/api/MuseumTile/GetMineData";
		_getGeneratedMineHttpRequest.Request(url);
	}
    
	private void OnGetMineDataRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var mine = JsonSerializer.Deserialize<global::ProjectMuseum.Models.Mine>(jsonStr);

		GD.Print("GET REQUEST COMPLETED");
		// var cells = mine.Cells;
		// foreach (var cell in cells)
		// {
		// 	GD.Print($"retrieved cells: {cell.Id}");
		// }
		GenerateMineBasedOnRetrievedMineData(mine);
	}

	private Cell[,] CellsListTo2DArray(List<Cell> cells, int length, int width, int cellSize)
	{
		var grid = new Cell[width, length];
		for (int y = 0; y < length; y++)
		{
			for (int x = 0; x < width; x++)
			{
				grid[x, y] = cells[x*width + y];
			}
		}

		return grid;
	}

	private void GenerateMineBasedOnRetrievedMineData(global::ProjectMuseum.Models.Mine mine)
	{
		GD.Print("GENERATING CELL LIST TO 2D ARRAY");
		var grid = CellsListTo2DArray(mine.Cells, mine.GridLength, mine.GridWidth,mine.CellSize);
		_mineGenerationVariables.Cells = grid;
		_mineGenerationVariables.GridLength = mine.GridLength;
		_mineGenerationVariables.GridWidth = mine.GridWidth;
		_mineGenerationVariables.CellSize = mine.CellSize;

		foreach (var cell in _mineGenerationVariables.Cells)
		{
			var pos = new Vector2(cell.PositionX, cell.PositionY);
			var tilePos = _mineGenerationView.LocalToMap(pos);
			
			if(!cell.IsInstantiated)
				_mineGenerationView.SetCell(0, tilePos, 1,new Vector2I(4, 0));
			else
			{
				if(!cell.IsBreakable)
					_mineGenerationView.SetCell(0, tilePos, 1,new Vector2I(3, 0));
				else
				{
					if(cell.BreakStrength == 3)
						_mineGenerationView.SetCell(0, tilePos, 1,new Vector2I(0, 0));
					else if(cell.BreakStrength == 2)
						_mineGenerationView.SetCell(0, tilePos, 1,new Vector2I(1, 0));
					else if(cell.BreakStrength == 1)
						_mineGenerationView.SetCell(0, tilePos, 1,new Vector2I(2, 0));
					else
						_mineGenerationView.SetCell(0, tilePos, 1,new Vector2I(4, 0));
				}
			}
		}
	}

	#endregion

	#region Mine Generator

	private void GenerateGrid()
	{
		_mineGenerationVariables.Cells = new Cell[_mineGenerationVariables.GridWidth, _mineGenerationVariables.GridLength];

		for (var x = 0; x < _mineGenerationVariables.GridWidth; x++)
		{
			for (var y = 0; y < _mineGenerationVariables.GridLength; y++)
			{
				if (y == 0 || y == _mineGenerationVariables.GridLength -1)
				{
					if (y == 0 && x == 17)
					{
						_mineGenerationVariables.Cells[x, y] = BlankCell(x, y);
						continue;
					}
					
					_mineGenerationVariables.Cells[x, y] = InstantiateUnbreakableCell(x, y);
					continue;
				}

				if (x == 0 || x == _mineGenerationVariables.GridWidth -1)
				{
					_mineGenerationVariables.Cells[x, y] = InstantiateUnbreakableCell(x, y);
					continue;
				}

				_mineGenerationVariables.Cells[x, y] = InstantiateCell(x, y);
			}
		}
	}

	private Cell BlankCell(int width, int height)
	{
		var cell = new Cell
		{
			Id = $"cell({width},{height})",
			IsBreakable = false,
			IsInstantiated = false,
			BreakStrength = 1000,
			PositionX = width * _mineGenerationVariables.CellSize,
			PositionY =  height * _mineGenerationVariables.CellSize,
		};
		var pos = new Vector2(cell.PositionX, cell.PositionY);
		var tilePos = _mineGenerationView.LocalToMap(pos);
		_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(4,0));
		return cell;
	}

	private Cell InstantiateUnbreakableCell(int width, int height)
	{
		var cell = new Cell
		{
			Id = $"cell({width},{height})",
			IsBreakable = false,
			IsInstantiated = true,
			BreakStrength = 1000,
			PositionX = width * _mineGenerationVariables.CellSize,
			PositionY =  height * _mineGenerationVariables.CellSize,
		};
        
		var pos = new Vector2(cell.PositionX, cell.PositionY);
		var tilePos = _mineGenerationView.LocalToMap(pos);
		_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(3,0));
		return cell;
	}

	private Cell InstantiateCell(int width, int height)
	{
		var cell = new Cell
		{
			Id = $"cell({width},{height})",
			IsBreakable = true,
			IsInstantiated = true,
			BreakStrength = 3,
			PositionX = width * _mineGenerationVariables.CellSize,
			PositionY =  height * _mineGenerationVariables.CellSize
		};
        
		var pos = new Vector2(cell.PositionX, cell.PositionY);
		var tilePos = _mineGenerationView.LocalToMap(pos);
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
		var cell = _mineGenerationVariables.Cells[tilePos.X, tilePos.Y];
		if (!cell.IsBreakable)
		{
			GD.Print("Is not breakable");
			return;
		}
		//GD.Print($"cell strength: {cell.BreakStrength}");
		_mineGenerationVariables.Cells[tilePos.X, tilePos.Y].BreakStrength--;
		Math.Clamp(-_mineGenerationVariables.Cells[tilePos.X, tilePos.Y].BreakStrength, 0, 100);
		
		if (cell.BreakStrength >= 2)
			_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(1,0));
		else if (cell.BreakStrength >= 1)
			_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(2,0));
		else
			_mineGenerationView.SetCell(0,tilePos,1,new Vector2I(4,0));
	}
    
	#endregion
}