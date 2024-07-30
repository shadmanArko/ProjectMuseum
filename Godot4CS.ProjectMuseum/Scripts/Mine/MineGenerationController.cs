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
	private HttpRequest _generateProceduralMineHttpRequest;
	private HttpRequest _mineCrackCellMaterialHttpRequest;
	private HttpRequest _getAllRawArtifactDescriptiveHttpRequest;
	private HttpRequest _getAllRawArtifactFunctionalHttpRequest;
	private HttpRequest _getAllMinArtifactsHttpRequest;
	private HttpRequest _assignResourceToMineHttpRequest;
    
	private MineGenerationVariables _mineGenerationVariables;
	private PlayerControllerVariables _playerControllerVariables;
	private MineCellCrackMaterial _mineCellCrackMaterial;
	private RawArtifactDTO _rawArtifactDto;
	[Export] private MineGenerationView _mineGenerationView;

	[Export] private CanvasLayer _savingCanvas;
	[Export] private Node2D _mineBackGround;
	private LoadingBarManager _loadingBarManager;

	public override async void _EnterTree()
	{
		CreateHttpRequests();
		InitializeDiReferences();
		GetMineCrackMaterialData();
		GetAllRawArtifactDescriptiveData();
		GetAllRawArtifactFunctionalData();
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
		
		_generateMineHttpRequest = new HttpRequest();
		AddChild(_generateMineHttpRequest);
		_generateMineHttpRequest.RequestCompleted += OnGenerateMineDataHttpRequestCompleted;
		
		_generateProceduralMineHttpRequest = new HttpRequest();
		AddChild(_generateProceduralMineHttpRequest);
		_generateProceduralMineHttpRequest.RequestCompleted += OnGenerateProceduralMineHttpRequestCompleted;
		
		_mineCrackCellMaterialHttpRequest = new HttpRequest();
		AddChild(_mineCrackCellMaterialHttpRequest);
		_mineCrackCellMaterialHttpRequest.RequestCompleted += OnGetMineCrackCellMaterialHttpRequestCompleted;
		
		_getAllRawArtifactDescriptiveHttpRequest = new HttpRequest();
		AddChild(_getAllRawArtifactDescriptiveHttpRequest);
		_getAllRawArtifactDescriptiveHttpRequest.RequestCompleted += OnGetGetAllRawArtifactDescriptiveHttpRequestCompleted;
		
		_getAllRawArtifactFunctionalHttpRequest = new HttpRequest();
		AddChild(_getAllRawArtifactFunctionalHttpRequest);
		_getAllRawArtifactFunctionalHttpRequest.RequestCompleted += OnGetGetAllRawArtifactFunctionalHttpRequestCompleted;
		
		_getAllMinArtifactsHttpRequest = new HttpRequest();
		AddChild(_getAllMinArtifactsHttpRequest);
		_getAllMinArtifactsHttpRequest.RequestCompleted += OnGetAllMinArtifactsHttpRequestCompleted;
	}

	#endregion
    
	private void InitializeDiReferences()
	{
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineCellCrackMaterial = ServiceRegistry.Resolve<MineCellCrackMaterial>();
		_rawArtifactDto = ServiceRegistry.Resolve<RawArtifactDTO>();
	}

	#region Get Mine Crack Material From Server

	private void GetMineCrackMaterialData()
	{
		var url = ApiAddress.MineApiPath+"GetAllMineCellCrackMaterials";
		_mineCrackCellMaterialHttpRequest.Request(url);
	}
	
	private void OnGetMineCrackCellMaterialHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		var cellCrackMaterials = JsonSerializer.Deserialize<List<CellCrackMaterial>>(jsonStr);
		_mineCellCrackMaterial.CellCrackMaterials = cellCrackMaterials;
	}

	#endregion
	
	#region Get Raw Artifact Descriptive Data From Server

	private void GetAllRawArtifactDescriptiveData()
	{
		var url = ApiAddress.MineApiPath+"GetAllRawArtifactDescriptive";
		_getAllRawArtifactDescriptiveHttpRequest.Request(url);
	}
	
	private void OnGetGetAllRawArtifactDescriptiveHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		var rawArtifactDescriptiveList = JsonSerializer.Deserialize<List<RawArtifactDescriptive>>(jsonStr);
		_rawArtifactDto.RawArtifactDescriptives = rawArtifactDescriptiveList;
	}

	#endregion
    
	#region Get Raw Artifact Functional Data From Server

	private void GetAllRawArtifactFunctionalData()
	{
		var url = ApiAddress.MineApiPath+"GetAllRawArtifactFunctional";
		_getAllRawArtifactFunctionalHttpRequest.Request(url);
	}
	
	private void OnGetGetAllRawArtifactFunctionalHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		var rawArtifactFunctionalList = JsonSerializer.Deserialize<List<RawArtifactFunctional>>(jsonStr);
		_rawArtifactDto.RawArtifactFunctionals = rawArtifactFunctionalList;
	}

	#endregion
	
	#region Get Raw Artifact Functional Data From Server

	private void GetAllArtifactData()
	{
		var url = ApiAddress.MineApiPath+"GetAllMineArtifacts";
		_getAllMinArtifactsHttpRequest.Request(url);
	}
	
	private void OnGetAllMinArtifactsHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		var artifactsList = JsonSerializer.Deserialize<List<Artifact>>(jsonStr);
		_rawArtifactDto.Artifacts = artifactsList;
		MineActions.OnRawArtifactDTOInitialized?.Invoke();
	}

	#endregion


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

	#region Assign Artifacts To Mine

	private async Task GenerateProceduralMine()
	{
		// var url = ApiAddress.MineApiPath + "GenerateProceduralMine";
		// _generateProceduralMineHttpRequest.Request(url);

		var mine = await ReferenceStorage.Instance.ProceduralMineGenerationService.GenerateProceduralMine();
		GD.Print($"Mine cells count: {mine.Cells.Count}");
		GetAllArtifactData();
		GenerateGridFromMineData(mine);
	}
	
	private void OnGenerateProceduralMineHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		var mine = JsonSerializer.Deserialize<global::ProjectMuseum.Models.Mine>(jsonStr);
		GetAllArtifactData();
		GenerateGridFromMineData(mine);
	}

	#endregion

	#region Mine Generator

	private void GenerateMineData()
	{
		var url = ApiAddress.MineApiPath+"GenerateProceduralMine";
		GD.Print("generating procedural mine");
		_generateMineHttpRequest.Request(url);
	}
	
	private void OnGenerateMineDataHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		GenerateProceduralMine();
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
		var AStarNodes = new List<AStarNode>();
		foreach (var cell in _mineGenerationVariables.Mine.Cells)
		{
			var aStarNode = new AStarNode(cell.PositionX, cell.PositionY, null, 0f, 0f, cell.IsBroken);
			AStarNodes.Add(aStarNode);
		}

		_mineGenerationVariables.PathfindingNodes = AStarNodes;
		GD.Print("path finding nodes generated");
	}

	#endregion
}