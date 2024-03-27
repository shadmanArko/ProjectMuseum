using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.ParticleEffects;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;

using ProjectMuseum.Models;
using Resource = ProjectMuseum.Models.MIne.Resource;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Object_Generators;

public partial class MineResourceCollector : Node
{
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;

	private HttpRequest _sendResourceFromMineToInventoryHttpRequest;
	private HttpRequest _getInventoryHttpRequest;
	
	private RandomNumberGenerator _randomNumberGenerator;
	
	private Inventory _inventory;
	
	public override void _Ready()
	{
		CreateHttpRequests();
		InitializeDiInstaller();
		SubscribeToActions();
		_randomNumberGenerator = new RandomNumberGenerator();
	}

	private void CreateHttpRequests()
	{
		_sendResourceFromMineToInventoryHttpRequest = new HttpRequest();
		AddChild(_sendResourceFromMineToInventoryHttpRequest);
		_sendResourceFromMineToInventoryHttpRequest.RequestCompleted += OnSendResourceFromMineToInventoryHttpRequestCompleted;
		
		_getInventoryHttpRequest = new HttpRequest();
		AddChild(_getInventoryHttpRequest);
		_getInventoryHttpRequest.RequestCompleted += OnGetInventoryHttpRequestCompleted;
	}

	private void InitializeDiInstaller()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnSuccessfulDigActionCompleted += CheckResourceCollectionValidity;
	}
	
	#region Check Resource Collection Validity

	private void CheckResourceCollectionValidity()
	{
		var tilePos = _mineGenerationVariables.MineGenView.LocalToMap(_playerControllerVariables.Position);
		tilePos += _playerControllerVariables.MouseDirection;
		var cell = _mineGenerationVariables.GetCell(tilePos);
		
		if(cell.HitPoint > 0) return;
		if(!cell.HasResource) return;
		
		var url = ApiAddress.PlayerApiPath+"GetInventory";
		_getInventoryHttpRequest.CancelRequest();
		_getInventoryHttpRequest.Request(url);
	}
	   
	private async void OnGetInventoryHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		_inventory = JsonSerializer.Deserialize<Inventory>(jsonStr);
        
		if (_inventory == null)
		{
			GD.Print("INVENTORY IS NULL");
			return;
		}
        
		if (_inventory.OccupiedSlots.Count >= _inventory.SlotsUnlocked)
		{
			GD.PrintErr("No empty slots in inventory");
			await ReferenceStorage.Instance.MinePopUp.ShowPopUp("No empty slots in inventory");
		}
		else
		{
			GD.Print("adding resource to inventory");
			CollectResources();
		}
	}
    
	#endregion
    
	#region Instantiate Resource Objects

	private void CollectResources()
	{
		var tilePos = _mineGenerationVariables.MineGenView.LocalToMap(_playerControllerVariables.Position);
		tilePos += _playerControllerVariables.MouseDirection;
		var cell = _mineGenerationVariables.GetCell(tilePos);
		var resource = _mineGenerationVariables.Mine.Resources.FirstOrDefault(tempResource =>
			tempResource.PositionX == cell.PositionX && tempResource.PositionY == cell.PositionY);

		if (resource == null) return;
		SendResourceFromMineToInventory(resource.Id);
		cell.HasResource = false;
	}

	#endregion

	#region Collect Resource

	private void SendResourceFromMineToInventory(string resourceId)
	{
		var url = ApiAddress.MineApiPath + "SendResourceFromMineToInventory/" + resourceId;
		_sendResourceFromMineToInventoryHttpRequest.Request(url);
	}
	
	private void OnSendResourceFromMineToInventoryHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
	{
		MineActions.OnInventoryUpdate?.Invoke();
	}

	#endregion

	public override void _ExitTree()
	{
		MineActions.OnSuccessfulDigActionCompleted += CollectResources;
	}
}