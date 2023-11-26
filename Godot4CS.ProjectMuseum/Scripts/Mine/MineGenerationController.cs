using System.Text;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineGenerationController : Node2D
{
	private HttpRequest _saveGeneratedMineHttpRequest;
	private HttpRequest _loadGeneratedMineHttpRequest;
	private HttpRequest _generateMineHttpRequest;
    
	private MineGenerationVariables _mineGenerationVariables;
	private MineGenerationView _mineGenerationView;

	[Export] private CanvasLayer _savingCanvas;
	[Export] private Node2D _mineBackGround;
    
	public override void _Ready()
	{
        CreateHttpRequests();
		InitializeDiReferences();
		_mineGenerationView = GetNode<MineGenerationView>("Mine");
		_mineGenerationVariables.MineGenView = _mineGenerationView;
		_savingCanvas.Visible = false;
		LoadMineDataFromServer();
	}

	private void CreateHttpRequests()
	{
		_saveGeneratedMineHttpRequest = new HttpRequest();
		AddChild(_saveGeneratedMineHttpRequest);
		_saveGeneratedMineHttpRequest.RequestCompleted += OnSaveGeneratedMineHttpRequestComplete;
		
		_loadGeneratedMineHttpRequest = new HttpRequest();
		AddChild(_loadGeneratedMineHttpRequest);
		_loadGeneratedMineHttpRequest.RequestCompleted += OnLoadMineDataRequestCompleted;
		
		_generateMineHttpRequest = new HttpRequest();
		AddChild(_generateMineHttpRequest);
		_generateMineHttpRequest.RequestCompleted += OnGenerateMineDataHttpRequestCompleted;
	}
    
	private void InitializeDiReferences()
	{
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	public void GenerateMine()
	{
		InitializeDiReferences();
		_mineBackGround.Position = new Vector2(482, -107);
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustReleased("generateGrid"))
			GenerateMine();
		
		if(Input.IsActionJustReleased("saveGrid"))
			SaveMineDataIntoServer();
			
		if(Input.IsActionJustReleased("loadGrid"))
			LoadMineDataFromServer();
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
	private void OnSaveGeneratedMineHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print("ON SAVE GENERATED MINE HTTP REQUEST COMPLETE method called");
		_savingCanvas.Visible = false;
	}

	#endregion

	#region Load Mine Data From Server

	private void LoadMineDataFromServer()
	{
		var url = ApiAddress.MineApiPath+"GetMineData";
		_loadGeneratedMineHttpRequest.Request(url);
	}
	private void OnLoadMineDataRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var mine = JsonSerializer.Deserialize<global::ProjectMuseum.Models.Mine>(jsonStr);
		
		GenerateGridFromMineData(mine);
	}
    
	#endregion

	#region Mine Generator

	private void GenerateMineData()
	{
		var url = ApiAddress.MineApiPath+"GenerateMine";
		_generateMineHttpRequest.Request(url);
	}
	private void GenerateGridFromMineData(global::ProjectMuseum.Models.Mine mine)
	{
		_mineGenerationVariables.Mine = mine;
		var cellSize = mine.CellSize;
		foreach (var cell in mine.Cells)
		{
			var pos = new Vector2(cell.PositionX * cellSize, cell.PositionY * cellSize);
			var tilePos = _mineGenerationView.LocalToMap(pos);
			MineSetCellConditions.SetTileMapCell(tilePos, cell, _mineGenerationView);
		}
	}
	private void OnGenerateMineDataHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		var mine = JsonSerializer.Deserialize<global::ProjectMuseum.Models.Mine>(jsonStr);
		GD.Print(mine);
		GenerateGridFromMineData(mine);
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
    
	#endregion
}