using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Object_Generators;

public partial class MineResourceCollector : Node
{
	private MineGenerationVariables _mineGenerationVariables;
	
	private InventoryDTO _inventoryDto;
	
	public override void _Ready()
	{
		InitializeDiInstaller();
		SubscribeToActions();
	}

	#region Initializers

	private void InitializeDiInstaller()
	{
		ServiceRegistry.Resolve<PlayerControllerVariables>();
		_mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
		_inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnMineCellBroken += CheckResourceCollectionValidity;
	}

	#endregion
	
	#region Check Resource Collection Validity

	private void CheckResourceCollectionValidity(Vector2I tilePos)
	{
		var cell = _mineGenerationVariables.GetCell(tilePos);
		if(cell.HitPoint > 0) return;
		if(!cell.HasResource) return;
		var cellSize = _mineGenerationVariables.Mine.CellSize;
		var cellOffset = new Vector2I(cellSize, cellSize) / 2;
		var globalTilePos = tilePos * cellSize + cellOffset;
		ReferenceStorage.Instance.InventoryItemBuilder.BuildInventoryItem("Resource", globalTilePos);
    }
    
	#endregion

	#region Finalizer

	private void UnsubscribeToActions()
	{
		MineActions.OnMineCellBroken -= CheckResourceCollectionValidity;
	}
	
	public override void _ExitTree()
	{
		UnsubscribeToActions();
	}

	#endregion
}