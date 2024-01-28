using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using Equipables = Godot4CS.ProjectMuseum.Scripts.Mine.Enum.Equipables;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class ToolbarSelector : Node
{
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
		SetEquipablesOnInventorySlots();
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
			_inventory.EmptySlots.Add(i);
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

	private void SetEquipablesOnInventorySlots()
	{
		foreach (var inventoryItem in _inventory.InventoryItems)
		{
			_inventory.EmptySlots.Remove(inventoryItem.Slot);
			_toolbarSlots[inventoryItem.Slot].SetItemTexture(inventoryItem.PngPath);
			_toolbarSlots[inventoryItem.Slot].SetItemData(inventoryItem.Id, false);
		}
	}

	private void SetArtifactsOnInventorySlots()
	{
		if(_inventory.Artifacts.Count <= 0) return;
		foreach (var artifact in _inventory.Artifacts)
		{
			var emptySlot = 0;

			for (var i = 0; i < _inventory.SlotsUnlocked; i++)
			{
				if(!_inventory.EmptySlots.Contains(i)) continue;
				emptySlot = i;
			}
			
			if(emptySlot == 0) continue;
			
			artifact.Slot = emptySlot;
			_inventory.EmptySlots.Remove(emptySlot);
			
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
    
	private void SelectItem(Equipables equipable)
	{
		DeselectAllItems();
		_toolbarSlots[(int) equipable].SetItemAsSelected();
	}


	private void DeselectAllItems()
	{
		foreach (var slot in _toolbarSlots)
			slot.SetItemAsDeselected();
	}
}