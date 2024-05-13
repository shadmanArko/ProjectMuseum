using System;
using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Object_Generators;

public partial class MineResourceCollector : Node
{
	private PlayerControllerVariables _playerControllerVariables;
	private MineGenerationVariables _mineGenerationVariables;
	
	private RandomNumberGenerator _randomNumberGenerator;
	
	private InventoryDTO _inventoryDto;
	
	public override void _Ready()
	{
		InitializeDiInstaller();
		SubscribeToActions();
		_randomNumberGenerator = new RandomNumberGenerator();
	}

	private void InitializeDiInstaller()
	{
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
		_inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
	}

	private void SubscribeToActions()
	{
		// MineActions.OnSuccessfulDigActionCompleted += CheckResourceCollectionValidity;
		MineActions.OnCollectItemDrop += CheckResourceCollectionValidity;
	}
	
	#region Check Resource Collection Validity

	private void CheckResourceCollectionValidity()
	{
		var tilePos = _mineGenerationVariables.MineGenView.LocalToMap(_playerControllerVariables.Position);
		tilePos += _playerControllerVariables.MouseDirection;
		var cell = _mineGenerationVariables.GetCell(tilePos);
		
		if(cell.HitPoint > 0) return;
		if(!cell.HasResource) return;
        
		if (_inventoryDto == null)
		{
			GD.Print("INVENTORY IS NULL");
			return;
		}
        
		if (_inventoryDto.Inventory.OccupiedSlots.Count >= _inventoryDto.Inventory.SlotsUnlocked)
		{
			GD.PrintErr("No empty slots in inventory");
			ReferenceStorage.Instance.MinePopUp.ShowPopUp("No empty slots in inventory");
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
		var item = SendResourceFromMineToInventory(resource.Id);
		cell.HasResource = false;
		MineActions.OnInventoryUpdate?.Invoke();
	}

	#endregion

	#region Collect Resource

	private InventoryItem SendResourceFromMineToInventory(string resourceId)
	{
		var inventoryManager = ReferenceStorage.Instance.InventoryManager;

		if (!inventoryManager.HasFreeSlot())
		{
			GD.PrintErr("Does not have free slot in inventory");
			return null;
		}
        
		var mine = _mineGenerationVariables.Mine;
		var resources = _mineGenerationVariables.Mine.Resources;
		var resourceToRemove = resources.FirstOrDefault(res => res.Id == resourceId);
		if (resourceToRemove != null)
			resources.Remove(resourceToRemove);
		else
		{
			GD.PrintErr("Resource is null");
			return null;
		}
		var cell = mine?.Cells.FirstOrDefault(cell => cell.PositionX == resourceToRemove.PositionX && cell.PositionY == resourceToRemove.PositionY);
		if (cell != null) cell.HasResource = false;
        
		InventoryItem item;
		if (_inventoryDto.Inventory.InventoryItems.Any(item1 => item1.Variant == resourceToRemove.Variant))
		{
			item = _inventoryDto.Inventory.InventoryItems.FirstOrDefault(item1 => item1.Variant == resourceToRemove.Variant)!;
			item.Stack++;
		}
		else
		{
			item = new InventoryItem
			{
				Id = Guid.NewGuid().ToString(),
				IsStackable = true,
				Name = resourceToRemove.Variant,
				Stack = 1,
				Slot = inventoryManager.GetNextEmptySlot(),
				Type = resourceToRemove.Type,
				Variant = resourceToRemove.Variant,
				PngPath = resourceToRemove.PNGPath
			};
            
			_inventoryDto.Inventory.OccupiedSlots.Add(item.Slot);
			_inventoryDto.Inventory.InventoryItems.Add(item);
		}

		return item;
	}

	#endregion

	private void UnsubscribeToActions()
	{
		MineActions.OnCollectItemDrop -= CheckResourceCollectionValidity;
	}
	
	public override void _ExitTree()
	{
		UnsubscribeToActions();
	}
}