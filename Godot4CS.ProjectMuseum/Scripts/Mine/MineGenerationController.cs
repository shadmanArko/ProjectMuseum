using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Loading_Bar;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineGenerationController : Node2D
{
	private HttpRequest _saveGeneratedMineHttpRequest;
	private HttpRequest _loadGeneratedMineHttpRequest;
	private HttpRequest _generateMineHttpRequest;
	
	private HttpRequest _mineCrackCellMaterialHttpRequest;
	private HttpRequest _getAllRawArtifactDescriptiveHttpRequest;
	private HttpRequest _getAllRawArtifactFunctionalHttpRequest;
	private HttpRequest _getAllMinArtifactsHttpRequest;
	private HttpRequest _assignResourceToMineHttpRequest;
    
	private MineGenerationVariables _mineGenerationVariables;
	private PlayerControllerVariables _playerControllerVariables;
	private RawArtifactDTO _rawArtifactDto;
	[Export] private MineGenerationView _mineGenerationView;

	[Export] private CanvasLayer _savingCanvas;
	[Export] private Node2D _mineBackGround;
	private LoadingBarManager _loadingBarManager;

	public override async void _EnterTree()
	{
		CreateHttpRequests();
		InitializeDiReferences();
		// GetMineCrackMaterialData();
		// GetAllRawArtifactDescriptiveData();
		// GetAllRawArtifactFunctionalData();
		await GenerateProceduralMine();
		
	}

	public override void _Ready()
	{
		_loadingBarManager = ReferenceStorage.Instance.LoadingBarManager;
		_mineGenerationVariables.MineGenView = _mineGenerationView;
		_savingCanvas.Visible = false;
		
		_mineBackGround.Position = new Vector2(482, -107);
		_loadingBarManager.EmitSignal("IncreaseRegisteredTask");
	}

	#region Http Requests

	private void CreateHttpRequests()
	{
		_saveGeneratedMineHttpRequest = new HttpRequest();
		AddChild(_saveGeneratedMineHttpRequest);
		_saveGeneratedMineHttpRequest.RequestCompleted += OnSaveGeneratedMineHttpRequestComplete;
		
		_loadGeneratedMineHttpRequest = new HttpRequest();
		AddChild(_loadGeneratedMineHttpRequest);
		_loadGeneratedMineHttpRequest.RequestCompleted += OnLoadMineDataRequestCompleted;
	}

	#endregion
    
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

	private async Task GenerateProceduralMine()
	{
		
		var mine = await ReferenceStorage.Instance.ProceduralMineGenerationService.GenerateProceduralMine();
		GD.Print($"Mine cells count: {mine.Cells.Count}");
		GenerateGridFromMineData(mine);
	}
	
	private void GenerateGridFromMineData(global::ProjectMuseum.Models.Mine mine)
	{
		_mineGenerationVariables.Mine = mine;
		var cellSize = mine.CellSize;
		foreach (var cell in mine.Cells)
		{
			var pos = new Vector2(cell.PositionX * cellSize, cell.PositionY * cellSize);
			var tilePos = _mineGenerationView.LocalToMap(pos);
			var cellCrackMaterial = new CellCrackMaterial();
			MineSetCellConditions.SetTileMapCell(_playerControllerVariables.MouseDirection, cell, cellCrackMaterial, _mineGenerationVariables);
		}

		MineSetCellConditions.SetBackdropDuringMineGeneration(_mineGenerationVariables);
		GenerateAStarPathfindingNodes();
		MineActions.OnMineGenerated?.Invoke();
		_loadingBarManager.EmitSignal("IncreaseCompletedTask");
	}
    
	#endregion

	#region Astar Pathfinding Node Generator

	private void GenerateAStarPathfindingNodes()
	{
		var aStarNodes = new List<AStarNode>();
		foreach (var cell in _mineGenerationVariables.Mine.Cells)
		{
			var aStarNode = new AStarNode(cell.PositionX, cell.PositionY, null, 0f, 0f, cell.IsBroken);
			aStarNodes.Add(aStarNode);
		}

		_mineGenerationVariables.PathfindingNodes = aStarNodes;
		GD.Print("path finding nodes generated");
	}

	#endregion
}