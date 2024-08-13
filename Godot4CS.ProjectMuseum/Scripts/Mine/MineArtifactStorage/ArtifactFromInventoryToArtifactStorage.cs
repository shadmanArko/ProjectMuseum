using System.Linq;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using ProjectMuseum.DTOs;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MineArtifactStorage;

public partial class ArtifactFromInventoryToArtifactStorage : Node2D
{
	private InventoryDTO _inventoryDto;

	public override void _Ready()
	{
		SubscribeToActions();
		InitializeDiReference();
	}

	private async void InitializeDiReference()
	{
		await Task.Delay(2000);
		_inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnPlayerReachedBackToCamp += SendArtifactFromInventoryToStorage;
	}
    
	#region From DI Inventory to Artifact Storage (New)

	private void SendArtifactFromInventoryToStorage()
	{
		var itemsToRemove = _inventoryDto.Inventory.InventoryItems.Where(item => item.Type == "Artifact").ToList();
		foreach (var item in itemsToRemove)
		{
			_inventoryDto.Inventory.InventoryItems.Remove(item);
		}

		foreach (var artifact in _inventoryDto.Inventory.Artifacts)
		{
			_inventoryDto.ArtifactStorage.Artifacts.Add(artifact);
			GD.Print($"added {artifact.RawArtifactId} to artifact storage");
		}
		
		_inventoryDto.Inventory.Artifacts.Clear();
	}

	#endregion

	private void UnsubscribeToActions()
	{
		MineActions.OnPlayerReachedBackToCamp -= SendArtifactFromInventoryToStorage;
	}
	
	public override void _ExitTree()
	{
		UnsubscribeToActions();
	}
}