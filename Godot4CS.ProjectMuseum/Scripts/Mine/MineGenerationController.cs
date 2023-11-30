using System.Text;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineGenerationController : Node2D
{
	private HttpRequest _saveGeneratedMineHttpRequest;
	private HttpRequest _loadGeneratedMineHttpRequest;
	private HttpRequest _generateMineHttpRequest;
	private HttpRequest _assignArtifactsToMineHttpRequest;
    
	private MineGenerationVariables _mineGenerationVariables;
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationView _mineGenerationView;

	[Export] private CanvasLayer _savingCanvas;
	[Export] private Node2D _mineBackGround;

	public override void _EnterTree()
	{
		InitializeDiReferences();
	}

	public override void _Ready()
	{
		CreateHttpRequests();
		InitializeDiReferences();
		_mineGenerationView = GetNode<MineGenerationView>("Mine");
		_mineGenerationVariables.MineGenView = _mineGenerationView;
		_savingCanvas.Visible = false;
		GenerateMineData();
		_mineBackGround.Position = new Vector2(482, -107);
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
		
		_assignArtifactsToMineHttpRequest = new HttpRequest();
		AddChild(_assignArtifactsToMineHttpRequest);
		_assignArtifactsToMineHttpRequest.RequestCompleted += OnAssignArtifactsToMineHttpRequestCompleted;
	}

	private void InitializeDiReferences()
	{
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
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

	#region Assign Artifacts To Mine

	private void AssignArtifactsToMine()
	{
		var url = ApiAddress.MineApiPath+"AssignArtifactsToMine";
		_assignArtifactsToMineHttpRequest.Request(url);
	}
	
	private void OnAssignArtifactsToMineHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		var mine = JsonSerializer.Deserialize<global::ProjectMuseum.Models.Mine>(jsonStr);
		GD.Print(mine);
		GenerateGridFromMineData(mine);
	}

	#endregion

	#region Mine Generator

	private void GenerateMineData()
	{
		var url = ApiAddress.MineApiPath+"GenerateMine";
		_generateMineHttpRequest.Request(url);
	}
	
	private void OnGenerateMineDataHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		AssignArtifactsToMine();
	}
	
	private void GenerateGridFromMineData(global::ProjectMuseum.Models.Mine mine)
	{
		_mineGenerationVariables.Mine = mine;
		var cellSize = mine.CellSize;
		foreach (var cell in mine.Cells)
		{
			var pos = new Vector2(cell.PositionX * cellSize, cell.PositionY * cellSize);
			var tilePos = _mineGenerationView.LocalToMap(pos);
			MineSetCellConditions.SetTileMapCell(tilePos, _playerControllerVariables.MouseDirection, cell, _mineGenerationView);
		}
	}
    
	#endregion

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionReleased("generateGrid"))
		{
			var cell = _mineGenerationVariables.GetCell(new Vector2I(24, 10));
			cell.HasArtifact = true;
			GD.Print("Generated artifact");
		}
		
		if(@event.IsActionReleased("saveGrid"))
			SaveMineDataIntoServer();
			
		if(@event.IsActionReleased("loadGrid"))
			LoadMineDataFromServer();
	}
}