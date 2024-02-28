using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class ToolbarSelector : Node
{
	private PlayerControllerVariables _playerControllerVariables;
	
	private RawArtifactDTO _rawArtifactDto;
    
	private Inventory _inventory;
	private HttpRequest _getPlayerInventoryHttpRequest;

	[Export] private string _toolbarSlotScenePath;
	
	private List<ToolbarSlot> _toolbarSlots;

	public override void _EnterTree()
	{
		CreateHttpRequest();
		InitializeDiReferences();
	}

	public override void _Ready()
	{
		SubscribeToActions();
		_toolbarSlots = new List<ToolbarSlot>();
	}
	
	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_rawArtifactDto = ServiceRegistry.Resolve<RawArtifactDTO>();
	}

	private void CreateHttpRequest()
	{
		_getPlayerInventoryHttpRequest = new HttpRequest();
		AddChild(_getPlayerInventoryHttpRequest);
		_getPlayerInventoryHttpRequest.RequestCompleted += OnGetPlayerInventoryHttpRequestComplete;
	}
	
	private void SubscribeToActions()
	{
		MineActions.OnToolbarSlotChanged += SelectItem;
		MineActions.OnInventoryUpdate += UpdatePlayerInventory;
		MineActions.OnRawArtifactDTOInitialized += GetPlayerInventory;
	}

	#region Player Inventory

	#region Get Player Inventory

	private void GetPlayerInventory()
	{
		var url = ApiAddress.PlayerApiPath+"GetInventory";
		_getPlayerInventoryHttpRequest.CancelRequest();
		_getPlayerInventoryHttpRequest.Request(url);
	}
	
	private void OnGetPlayerInventoryHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		_inventory = JsonSerializer.Deserialize<Inventory>(jsonStr);
		
		RemoveAllInventorySlots();
		CreateInventorySlots();
		SetArtifactsOnInventorySlots();
		SetInventoryItemSlots();
		SelectItem(_playerControllerVariables.CurrentEquippedItemSlot);
	}

	#endregion

	private void UpdatePlayerInventory()
	{
		GetPlayerInventory();
		GD.Print("UPDATING PLAYER INVENTORY");
	}

	private void CreateInventorySlots()
	{
		for (var i = 0; i < _inventory.SlotsUnlocked; i++)
		{
			var toolbarSlot = ResourceLoader.Load<PackedScene>(_toolbarSlotScenePath).Instantiate() as ToolbarSlot;
			if (toolbarSlot == null)
			{
				GD.PrintErr($"error instantiating slot");
				continue;
			}
            
			AddChild(toolbarSlot);
			_toolbarSlots.Add(toolbarSlot);
		}
	}

	private void RemoveAllInventorySlots()
	{
		if(_toolbarSlots.Count <= 0) return;
		foreach (var t in _toolbarSlots)
			t.QueueFree();
		_toolbarSlots.Clear();
		_toolbarSlots = new List<ToolbarSlot>();
	}

	private void SetInventoryItemSlots()
	{
		foreach (var inventoryItem in _inventory.InventoryItems)
		{
			_toolbarSlots[inventoryItem.Slot].SetItemTexture(inventoryItem.PngPath);
			_toolbarSlots[inventoryItem.Slot].SetItemData(inventoryItem.Id, false);
			
			if(inventoryItem.Stack > 1)
				_toolbarSlots[inventoryItem.Slot].TurnOnItemCounter(inventoryItem.Stack);
			else
				_toolbarSlots[inventoryItem.Slot].TurnOffItemCounter();
		}
	}

	private void SetArtifactsOnInventorySlots()
	{
		if(_inventory.Artifacts.Count <= 0) return;
		foreach (var artifact in _inventory.Artifacts)
		{
			var rawArtifactFunctional =
				_rawArtifactDto.RawArtifactFunctionals.FirstOrDefault(rawArtifactFunctional => rawArtifactFunctional.Id == artifact.RawArtifactId);
			if (rawArtifactFunctional == null)
			{
				GD.Print("Artifact Not Found");
				continue;
			}
			_toolbarSlots[artifact.Slot].SetItemTexture(rawArtifactFunctional.SmallImageLocation);
			_toolbarSlots[artifact.Slot].SetItemData(rawArtifactFunctional.Id, true);
		}
	}

	private void RemoveAllArtifactsFromInventorySlots()
	{
		foreach (var slot in _toolbarSlots)
			slot.RemoveItem();
	}

	#endregion
    
	private void SelectItem(int itemNumber)
	{
		GD.Print($"current item selected in toolbar: {itemNumber}");
		DeselectAllItems();
		_toolbarSlots[itemNumber].SetItemAsSelected();
	}


	private void DeselectAllItems()
	{
		foreach (var slot in _toolbarSlots)
			slot.SetItemAsDeselected();
	}
}