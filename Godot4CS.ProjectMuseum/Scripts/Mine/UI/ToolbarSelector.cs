using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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
    
	private InventoryDTO _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
	private HttpRequest _getPlayerInventoryHttpRequest;

	[Export] private string _toolbarSlotScenePath;
	
	private List<ToolbarSlot> _toolbarSlots;

	#region Initializers

	public override async void _Ready()
	{
		InitializeDiReferences();
		SubscribeToActions();
		_toolbarSlots = new List<ToolbarSlot>();
		await Task.Delay(5000);
		UpdateToolbar();
	}
	
	private void InitializeDiReferences()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_rawArtifactDto = ServiceRegistry.Resolve<RawArtifactDTO>();
	}
	
	private void SubscribeToActions()
	{
		MineActions.OnToolbarSlotChanged += SelectItem;
		MineActions.OnInventoryUpdate += UpdateToolbar;
		MineActions.OnRawArtifactDTOInitialized += UpdateToolbar;
	}

	#endregion
    
	#region Player Inventory

	#region Get Player Inventory

	private void RefreshToolbar()
	{
		RemoveAllToolbarSlots();
		CreateToolbarSlots();
		SetToolbarItemSlots();
		SelectItem(_playerControllerVariables.CurrentEquippedItemSlot);
	}

	#endregion

	private void UpdateToolbar()
	{
		GD.Print("UPDATING PLAYER INVENTORY");
		RefreshToolbar();
	}

	private void CreateToolbarSlots()
	{
		for (var i = 0; i < 12; i++)
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

	private void RemoveAllToolbarSlots()
	{
		if(_toolbarSlots.Count <= 0) return;
		foreach (var t in _toolbarSlots)
			t.QueueFree();
		_toolbarSlots.Clear();
		_toolbarSlots = new List<ToolbarSlot>();
	}

	private void SetToolbarItemSlots()
	{
		foreach (var inventoryItem in _inventoryDto.Inventory.InventoryItems)
		{
			if (inventoryItem.Slot is < 0 or >= 12) continue;
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
		if(_inventoryDto.Inventory.Artifacts.Count <= 0) return;
		foreach (var artifact in _inventoryDto.Inventory.Artifacts)
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

	#region Select and Deselect

	private void SelectItem(int itemNumber)
	{
		GD.Print($"current item selected in toolbar: {itemNumber}");
		DeselectAllItems();
		GD.Print($"item number: {itemNumber}");
		GD.Print($"toolbar slots: {_toolbarSlots.Count}");
		_toolbarSlots[itemNumber].SetItemAsSelected();
	}


	private void DeselectAllItems()
	{
		foreach (var slot in _toolbarSlots)
			slot.SetItemAsDeselected();
	}

	#endregion

	#region Exit Tree

	private void UnsubscribeToActions()
	{
		MineActions.OnToolbarSlotChanged -= SelectItem;
		MineActions.OnInventoryUpdate -= UpdateToolbar;
		MineActions.OnRawArtifactDTOInitialized -= UpdateToolbar;
	}

	public override void _ExitTree()
	{
		UnsubscribeToActions();
	}

	#endregion
}