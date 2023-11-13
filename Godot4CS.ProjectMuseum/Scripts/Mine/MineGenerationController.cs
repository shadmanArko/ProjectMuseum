using System.Collections.Generic;
using System.Text;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineGenerationController : Node2D
{
	private global::ProjectMuseum.Models.Mine _mine = new();
	
	private HttpRequest _saveGeneratedMineHttpRequest;
	private HttpRequest _getGeneratedMineHttpRequest;
	
	private MineGenerationView _mineGenerationView;

	private MineGenerationVariables _mineGenerationVariables;

	[Export] private CanvasLayer _savingCanvas;
    
	public override void _Ready()
	{
		_saveGeneratedMineHttpRequest = new HttpRequest();
		AddChild(_saveGeneratedMineHttpRequest);
		_saveGeneratedMineHttpRequest.RequestCompleted += OnSaveGeneratedMineHttpRequestComplete;
		
		_getGeneratedMineHttpRequest = new HttpRequest();
		AddChild(_getGeneratedMineHttpRequest);
		_getGeneratedMineHttpRequest.RequestCompleted += OnGetMineDataRequestCompleted;
		
		InitializeDiReferences();
		_mineGenerationView = GetNode<MineGenerationView>("Mine");
		_mineGenerationVariables.MineGenView = _mineGenerationView;
		_savingCanvas.Visible = false;
	}

	private void InitializeDiReferences()
	{
		ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	public void GenerateMine()
	{
		InitializeDiReferences();
		GenerateGrid();
        
		_mineBackGround.Position = new Vector2(482, -107);
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
		_mine.CellSize = _mineGenerationVariables.CellSize;
		_mine.GridLength = _mineGenerationVariables.GridLength;
		_mine.GridWidth = _mineGenerationVariables.GridWidth;
		
		var body = JsonConvert.SerializeObject(_mine);

		_saveGeneratedMineHttpRequest.Request(ApiAddress.MineApiPath+"UpdateMineData", headers,
			HttpClient.Method.Put, body);
		_savingCanvas.Visible = true;
	}
	
	private List<Cell> Cells2DArrayToList()
	{
		var cellList = new List<Cell>();
		for (var x = 0; x < _mineGenerationVariables.GridWidth; x++)
		{
			for (var y = 0; y < _mineGenerationVariables.GridLength; y++)
				cellList.Add(_mineGenerationVariables.Cells[x,y]);
		}

		return cellList;
	}
	
	private void OnSaveGeneratedMineHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print("ON SAVE GENERATED MINE HTTP REQUEST COMPLETE method called");
		_savingCanvas.Visible = false;
	}

	#endregion

	#region Get Mine Data From Server

	private void GetMineDataFromServer()
	{
		GD.Print("RETRIEVING CELL LIST");
		var url = ApiAddress.MineApiPath+"GetMineData";
		_getGeneratedMineHttpRequest.Request(url);
	}
    
	private void OnGetMineDataRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var mine = JsonSerializer.Deserialize<global::ProjectMuseum.Models.Mine>(jsonStr);
		
		GD.Print("GET REQUEST COMPLETED");
		GenerateMineBasedOnRetrievedMineData(mine);
	}
    
	private void GenerateMineBasedOnRetrievedMineData(global::ProjectMuseum.Models.Mine mine)
	{
		GD.Print("GENERATING CELL LIST TO 2D ARRAY");
		var grid = CellsListTo2DArray(mine.Cells, mine.GridLength, mine.GridWidth);
		_mineGenerationVariables.Cells = grid;
		_mineGenerationVariables.GridLength = mine.GridLength;
		_mineGenerationVariables.GridWidth = mine.GridWidth;
		_mineGenerationVariables.CellSize = mine.CellSize;

		foreach (var cell in _mineGenerationVariables.Cells)
		{
			var pos = new Vector2(cell.PositionX, cell.PositionY);
			var tilePos = _mineGenerationView.LocalToMap(pos);
			
			if(!cell.IsInstantiated)
				_mineGenerationView.SetCell(0, tilePos, _mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(4, 0));
			else
			{
				if(!cell.IsBreakable || !cell.IsRevealed)
					_mineGenerationView.SetCell(0, tilePos, _mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(3, 0));
				else
				{
					if(cell.BreakStrength == 3)
						_mineGenerationView.SetCell(0, tilePos, _mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(0, 0));
					else if(cell.BreakStrength == 2)
						_mineGenerationView.SetCell(0, tilePos, _mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(1, 0));
					else if(cell.BreakStrength == 1)
						_mineGenerationView.SetCell(0, tilePos, _mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(2, 0));
					else
						_mineGenerationView.SetCell(0, tilePos, _mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(4, 0));
				}
			}
		}
	}
	
	private static Cell[,] CellsListTo2DArray(List<Cell> cells, int length, int width)
	{
		var grid = new Cell[width, length];
		var listIndex = 0;
		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < length; y++)
			{
				grid[x, y] = cells[listIndex];
				listIndex++;
			}
		}

		return grid;
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
					if (y is 0 && x == _mineGenerationVariables.GridWidth / 2)
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
				if (y is 1 && x == _mineGenerationVariables.GridWidth / 2)
					_mineGenerationVariables.Cells[x, y].IsRevealed = true;
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
		_mineGenerationView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(4,0));
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
		_mineGenerationView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(3,0));
		return cell;
	}

	private Cell InstantiateCell(int width, int height)
	{
		var cell = new Cell
		{
			Id = $"cell({width},{height})",
			IsBreakable = true,
			IsInstantiated = true,
			IsRevealed = false,
			BreakStrength = 3,
			PositionX = width * _mineGenerationVariables.CellSize,
			PositionY =  height * _mineGenerationVariables.CellSize
		};
        
		var pos = new Vector2(cell.PositionX, cell.PositionY);
		var tilePos = _mineGenerationView.LocalToMap(pos);
		_mineGenerationView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(3,0));

		return cell;
	}

	#endregion

	[Export] private Node2D _mineBackGround;
}