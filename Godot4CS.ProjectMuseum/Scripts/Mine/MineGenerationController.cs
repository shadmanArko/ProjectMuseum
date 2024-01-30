using System.Collections.Generic;
using System.Text;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
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
	private HttpRequest _assignArtifactsToMineHttpRequest;
	private HttpRequest _mineCrackCellMaterialHttpRequest;
	private HttpRequest _rawArtifactDescriptiveHttpRequest;
	private HttpRequest _rawArtifactFunctionalHttpRequest;
	private HttpRequest _assignResourceToMineHttpRequest;
    
	private MineGenerationVariables _mineGenerationVariables;
	private PlayerControllerVariables _playerControllerVariables;
	private MineCellCrackMaterial _mineCellCrackMaterial;
	private RawArtifactDTO _rawArtifactDto;
	[Export] private MineGenerationView _mineGenerationView;

	[Export] private CanvasLayer _savingCanvas;
	[Export] private Node2D _mineBackGround;

	public override void _EnterTree()
	{
		CreateHttpRequests();
		InitializeDiReferences();
		GetMineCrackMaterialData();
		GetAllRawArtifactDescriptiveData();
		GetAllRawArtifactFunctionalData();
		AssignArtifactsToMine();
	}

	public override void _Ready()
	{
		_mineGenerationView = GetNode<MineGenerationView>("Mine");
		_mineGenerationVariables.MineGenView = _mineGenerationView;
		_savingCanvas.Visible = false;
		
		_mineBackGround.Position = new Vector2(482, -107);
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
		
		_assignArtifactsToMineHttpRequest = new HttpRequest();
		AddChild(_assignArtifactsToMineHttpRequest);
		_assignArtifactsToMineHttpRequest.RequestCompleted += OnAssignArtifactsToMineHttpRequestCompleted;
		
		_mineCrackCellMaterialHttpRequest = new HttpRequest();
		AddChild(_mineCrackCellMaterialHttpRequest);
		_mineCrackCellMaterialHttpRequest.RequestCompleted += OnGetMineCrackCellMaterialHttpRequestCompleted;
		
		_rawArtifactDescriptiveHttpRequest = new HttpRequest();
		AddChild(_rawArtifactDescriptiveHttpRequest);
		_rawArtifactDescriptiveHttpRequest.RequestCompleted += OnGetRawArtifactDescriptiveHttpRequestCompleted;
		
		_rawArtifactFunctionalHttpRequest = new HttpRequest();
		AddChild(_rawArtifactFunctionalHttpRequest);
		_rawArtifactFunctionalHttpRequest.RequestCompleted += OnGetRawArtifactFunctionalHttpRequestCompleted;
		
		_assignResourceToMineHttpRequest = new HttpRequest();
		AddChild(_assignResourceToMineHttpRequest);
		_assignResourceToMineHttpRequest.RequestCompleted += OnAssignResourceToMineHttpRequestCompleted;
	}

	#endregion

	private void OnGetRawArtifactFunctionalHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		var rawArtifactFunctionalList = JsonSerializer.Deserialize<List<RawArtifactFunctional>>(jsonStr);
		_rawArtifactDto.RawArtifactFunctionals = rawArtifactFunctionalList;
		MineActions.OnRawArtifactDTOInitialized?.Invoke();
	}

	private void OnGetRawArtifactDescriptiveHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		var rawArtifactDescriptiveList = JsonSerializer.Deserialize<List<RawArtifactDescriptive>>(jsonStr);
		_rawArtifactDto.RawArtifactDescriptives = rawArtifactDescriptiveList;
	}

	private void OnGetMineCrackCellMaterialHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		var cellCrackMaterials = JsonSerializer.Deserialize<List<CellCrackMaterial>>(jsonStr);
		_mineCellCrackMaterial.CellCrackMaterials = cellCrackMaterials;
	}

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

	#endregion
	
	#region Get Raw Artifact Descriptive Data From Server

	private void GetAllRawArtifactDescriptiveData()
	{
		var url = ApiAddress.MineApiPath+"GetAllRawArtifactDescriptive";
		_rawArtifactDescriptiveHttpRequest.Request(url);
	}

	#endregion
    
	#region Get Raw Artifact Functional Data From Server

	private void GetAllRawArtifactFunctionalData()
	{
		var url = ApiAddress.MineApiPath+"GetAllRawArtifactFunctional";
		_rawArtifactFunctionalHttpRequest.Request(url);
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
		// GD.Print("ON SAVE GENERATED MINE HTTP REQUEST COMPLETE method called");
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
        
		AssignResourcesToMine();
		// GenerateGridFromMineData(mine);
	}

	#endregion

	#region Assign Resources To Mine

	private void AssignResourcesToMine()
	{
		var url = ApiAddress.MineApiPath+"AssignResourcesToMine";
		_assignResourceToMineHttpRequest.Request(url);
	}
	
	private void OnAssignResourceToMineHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
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
			var cellCrackMaterial = new CellCrackMaterial();
			MineSetCellConditions.SetTileMapCell(tilePos, _playerControllerVariables.MouseDirection, cell, cellCrackMaterial, _mineGenerationVariables);
		}
	}
    
	#endregion
}