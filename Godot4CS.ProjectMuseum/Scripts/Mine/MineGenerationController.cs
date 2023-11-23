using System.Collections.Generic;
using System.Text;
using Godot;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
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
	private HttpRequest _generateMineHttpRequest;
	
	private MineGenerationView _mineGenerationView;

	private MineGenerationVariables _mineGenerationVariables;

	[Export] private CanvasLayer _savingCanvas;
    
	public override void _Ready()
	{
        CreateHttpRequests();
		InitializeDiReferences();
		_mineGenerationView = GetNode<MineGenerationView>("Mine");
		_mineGenerationVariables.MineGenView = _mineGenerationView;
		_savingCanvas.Visible = false;
		GenerateMineData();
	}

	private void CreateHttpRequests()
	{
		_saveGeneratedMineHttpRequest = new HttpRequest();
		AddChild(_saveGeneratedMineHttpRequest);
		_saveGeneratedMineHttpRequest.RequestCompleted += OnSaveGeneratedMineHttpRequestComplete;
		
		_getGeneratedMineHttpRequest = new HttpRequest();
		AddChild(_getGeneratedMineHttpRequest);
		_getGeneratedMineHttpRequest.RequestCompleted += OnGetMineDataRequestCompleted;
		
		_generateMineHttpRequest = new HttpRequest();
		AddChild(_generateMineHttpRequest);
		_generateMineHttpRequest.RequestCompleted += GetGeneratedMineDataHttpRequestCompleted;
	}

	private void GetGeneratedMineDataHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var mine = JsonSerializer.Deserialize<global::ProjectMuseum.Models.Mine>(jsonStr);
		GD.Print(mine);
		GenerateGrid(mine);
	}

	private void InitializeDiReferences()
	{
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	public void GenerateMine()
	{
		InitializeDiReferences();
		//GenerateGrid();
        
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
		
		var body = JsonConvert.SerializeObject(_mineGenerationVariables.Mine);

		_saveGeneratedMineHttpRequest.Request(ApiAddress.MineApiPath+"UpdateMineData", headers,
			HttpClient.Method.Put, body);
		_savingCanvas.Visible = true;
	}
	
	// private List<Cell> Cells2DArrayToList()
	// {
	// 	var cellList = new List<Cell>();
	// 	for (var x = 0; x < _mineGenerationVariables.GridWidth; x++)
	// 	{
	// 		for (var y = 0; y < _mineGenerationVariables.GridLength; y++)
	// 			cellList.Add(_mineGenerationVariables.GetCell(new ));
	// 	}
	//
	// 	return cellList;
	// }
	
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
		// GD.Print("GENERATING CELL LIST TO 2D ARRAY");
		// var grid = CellsListTo2DArray(mine.Cells, mine.GridLength, mine.GridWidth);
		// _mineGenerationVariables.Cells = grid;
		// _mineGenerationVariables.GridLength = mine.GridLength;
		// _mineGenerationVariables.GridWidth = mine.GridWidth;
		// _mineGenerationVariables.CellSize = mine.CellSize;
		//
		// foreach (var cell in _mineGenerationVariables.Cells)
		// {
		// 	var pos = new Vector2(cell.PositionX, cell.PositionY);
		// 	var tilePos = _mineGenerationView.LocalToMap(pos);
		// 	
		// 	if(!cell.IsInstantiated)
		// 		_mineGenerationView.SetCell(0, tilePos, _mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(4, 0));
		// 	else
		// 	{
		// 		if(!cell.IsBreakable || !cell.IsRevealed)
		// 			_mineGenerationView.SetCell(0, tilePos, _mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(3, 0));
		// 		else
		// 		{
		// 			if(cell.HitPoint == 3)
		// 				_mineGenerationView.SetCell(0, tilePos, _mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(0, 0));
		// 			else if(cell.HitPoint == 2)
		// 				_mineGenerationView.SetCell(0, tilePos, _mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(1, 0));
		// 			else if(cell.HitPoint == 1)
		// 				_mineGenerationView.SetCell(0, tilePos, _mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(2, 0));
		// 			else
		// 				_mineGenerationView.SetCell(0, tilePos, _mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(4, 0));
		// 		}
		// 	}
		// }
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

	private void GenerateMineData()
	{
		GD.Print("GENERATING CELL LIST");
		var url = ApiAddress.MineApiPath+"GenerateMine";
		_generateMineHttpRequest.Request(url);
	}
	
	private void GenerateGrid(global::ProjectMuseum.Models.Mine mine)
	{
		_mineGenerationVariables.Mine = mine;
		//var cells = mine.Cells;
		//_mineGenerationVariables.Cells = CellsListTo2DArray(mine.Cells, mine.GridLength, mine.GridWidth);

		foreach (var cell in mine.Cells)
		{
			var pos = new Vector2(cell.PositionX * 20, cell.PositionY * 20);
			var tilePos = _mineGenerationView.LocalToMap(pos);
			GD.Print($"{cell.PositionX}, {cell.PositionY}");
			//_mineGenerationView.SetCell(0, tilePos, _mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(3, 0));
			MineSetCellConditions.SetTileMapCell(tilePos, cell, _mineGenerationView);
		}
	}

	// private void GenerateGrid()
	// {
	// 	_mineGenerationVariables.Cells = new Cell[_mineGenerationVariables.GridWidth, _mineGenerationVariables.GridLength];
	//
	// 	for (var x = 0; x < _mineGenerationVariables.GridWidth; x++)
	// 	{
	// 		for (var y = 0; y < _mineGenerationVariables.GridLength; y++)
	// 		{
	// 			if (y == 0 || y == _mineGenerationVariables.GridLength -1)
	// 			{
	// 				if (y is 0 && x == _mineGenerationVariables.GridWidth / 2)
	// 				{
	// 					_mineGenerationVariables.Cells[x, y] = BlankCell(x, y);
	// 					continue;
	// 				}
	// 				
	// 				_mineGenerationVariables.Cells[x, y] = InstantiateUnbreakableCell(x, y);
	// 				continue;
	// 			}
	//
	// 			if (x == 0 || x == _mineGenerationVariables.GridWidth -1)
	// 			{
	// 				_mineGenerationVariables.Cells[x, y] = InstantiateUnbreakableCell(x, y);
	// 				continue;
	// 			}
	//
	// 			if (x == 24 && y == 5)
	// 			{
	// 				_mineGenerationVariables.Cells[x, y] = InstantiateArtifactCell(x, y);
	// 				continue;
	// 			}
 //                
	// 			_mineGenerationVariables.Cells[x, y] = InstantiateCell(x, y);
	// 			if (y is 1 && x == _mineGenerationVariables.GridWidth / 2)
	// 				_mineGenerationVariables.Cells[x, y].IsRevealed = true;
	// 		}
	// 	}
	// }

	private Cell BlankCell(int width, int height)
	{
		var cell = new Cell
		{
			Id = $"cell({width},{height})",
			IsBreakable = false,
			IsInstantiated = false,
			HitPoint = 1000,
			PositionX = width * _mineGenerationVariables.CellSize,
			PositionY =  height * _mineGenerationVariables.CellSize,
		};
		var pos = new Vector2(cell.PositionX, cell.PositionY);
		var tilePos = _mineGenerationView.LocalToMap(pos);
		_mineGenerationView.SetCell(0,tilePos,5,new Vector2I(2,15));
		return cell;
	}

	private Cell InstantiateUnbreakableCell(int width, int height)
	{
		var cell = new Cell
		{
			Id = $"cell({width},{height})",
			IsBreakable = false,
			IsInstantiated = true,
			HitPoint = 1000,
			PositionX = width * _mineGenerationVariables.CellSize,
			PositionY =  height * _mineGenerationVariables.CellSize,
		};
        
		var pos = new Vector2(cell.PositionX, cell.PositionY);
		var tilePos = _mineGenerationView.LocalToMap(pos);
		_mineGenerationView.SetCell(0,tilePos,5,new Vector2I(1,15));
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
			HitPoint = 3,
			PositionX = width * _mineGenerationVariables.CellSize,
			PositionY =  height * _mineGenerationVariables.CellSize
		};
        
		var pos = new Vector2(cell.PositionX, cell.PositionY);
		var tilePos = _mineGenerationView.LocalToMap(pos);
		_mineGenerationView.SetCell(0,tilePos,5,new Vector2I(1,15));
		//_mineGenerationVariables.MineGenView.SetCellsTerrainConnect(0,new Array<Vector2I>(new []{tilePos}),0,0);
		return cell;
	}
	
	private Cell InstantiateArtifactCell(int width, int height)
	{
		var cell = new Cell
		{
			Id = $"cell({width},{height})",
			IsBreakable = true,
			IsInstantiated = true,
			IsRevealed = false,
			HasArtifact = true,
			HitPoint = 3,
			PositionX = width * _mineGenerationVariables.CellSize,
			PositionY =  height * _mineGenerationVariables.CellSize
		};
        
		var pos = new Vector2(cell.PositionX, cell.PositionY);
		var tilePos = _mineGenerationView.LocalToMap(pos);
		//_mineGenerationView.SetCell(0,tilePos,_mineGenerationVariables.MineGenView.TileSourceId,new Vector2I(3,0));
		_mineGenerationVariables.MineGenView.SetCellsTerrainConnect(0,new Array<Vector2I>(new []{tilePos}),0,0);
		return cell;
	}
    
	#endregion

	[Export] private Node2D _mineBackGround;
}