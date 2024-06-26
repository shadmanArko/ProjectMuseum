using System;
using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Objects;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using Resource = ProjectMuseum.Models.MIne.Resource;

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
		MineActions.OnMineCellBroken += CheckResourceCollectionValidity;
		
		MineActions.OnCollectItemDrop += SendInventoryItemToInventory;
	}
	
	#region Check Resource Collection Validity

	private void CheckResourceCollectionValidity(Vector2I tilePos)
	{
		var cell = _mineGenerationVariables.GetCell(tilePos);
		
		if(cell.HitPoint > 0) return;
		if(!cell.HasResource) return;
		
		var resource = _mineGenerationVariables.Mine.Resources.FirstOrDefault(tempResource =>
			tempResource.PositionX == cell.PositionX && tempResource.PositionY == cell.PositionY);
		if (resource == null)
		{
			GD.PrintErr($"Could not find resource in the mine");
			return;
		}

		cell.HasResource = false;
		var cellSize = _mineGenerationVariables.Mine.CellSize;
		var offset = new Vector2(cellSize, cellSize) / 2;
		var pos = new Vector2(cell.PositionX, cell.PositionY) * cellSize + offset;
		InstantiateResourceAsInventoryItem(resource, pos);
    }

	private void InstantiateResourceAsInventoryItem(Resource resource, Vector2 position)
	{
		var inventoryItem = new InventoryItem
		{
			Id = resource.Id,
			Type = "Resource",
			Category = resource.Category,
			Variant = resource.Variant,
			IsStackable = true,
			Name = resource.Name,
			Stack = 1,
			Slot = 0,
			PngPath = resource.PNGPath
		};

		var itemDropPath = ReferenceStorage.Instance.ItemDropScenePath;
		var resourceItem =
			SceneInstantiator.InstantiateScene(itemDropPath, _mineGenerationVariables.MineGenView, position) as ItemDrop;
		if (resourceItem == null)
		{
			GD.PrintErr("Item drop is null");
			return;
		}
		
		GD.Print("instantiated resource item");
		resourceItem.InventoryItem = inventoryItem;
	}

	private void SendInventoryItemToInventory(InventoryItem item)
	{
		if(item.Type != "Resource") return;
		var inventoryManager = ReferenceStorage.Instance.InventoryManager;
		var mine = _mineGenerationVariables.Mine;
		var resources = _mineGenerationVariables.Mine.Resources;
		var resourceToRemove = resources.FirstOrDefault(res => res.Id == item.Id);
		if (resourceToRemove != null)
			resources.Remove(resourceToRemove);
		else
		{
			GD.PrintErr("Resource is null");
			return;
		}
		var cell = mine?.Cells.FirstOrDefault(cell => cell.PositionX == resourceToRemove.PositionX && cell.PositionY == resourceToRemove.PositionY);
		if (cell != null) cell.HasResource = false;
		
		if (_inventoryDto.Inventory.InventoryItems.Any(item1 => item1.Variant == resourceToRemove.Variant))
		{
			item = _inventoryDto.Inventory.InventoryItems.FirstOrDefault(item1 => item1.Variant == resourceToRemove.Variant)!;
			item.Stack++;
		}
		else
		{
			item.Slot = inventoryManager.GetNextEmptySlot();
			_inventoryDto.Inventory.OccupiedSlots.Add(item.Slot);
			_inventoryDto.Inventory.InventoryItems.Add(item);
		}
		MineActions.OnInventoryUpdate?.Invoke();
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
		MineActions.OnMineCellBroken -= CheckResourceCollectionValidity;
		MineActions.OnCollectItemDrop -= SendInventoryItemToInventory;
	}
	
	public override void _ExitTree()
	{
		UnsubscribeToActions();
	}
}