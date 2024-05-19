using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Objects;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.ArtifactStorage;

public partial class ArtifactFromMineToInventory : Node2D
{
    private InventoryDTO _inventoryDto;
    private MineGenerationVariables _mineGenerationVariables;
    private RawArtifactDTO _rawArtifactDto;

    #region Initializers

    public override void _Ready()
    {
        SubscribeToActions();
        InitializeDiReference();
    }

    private void InitializeDiReference()
    {
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
        _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
        _rawArtifactDto = ServiceRegistry.Resolve<RawArtifactDTO>();
    }
    
    private void SubscribeToActions()
    {
        MineActions.OnArtifactSuccessfullyRetrieved += RemoveFromMineAndInstantiateInventoryItem;
        MineActions.OnCollectItemDrop += SendArtifactItemToInventory;
    }

    #endregion

    #region Remove Artifact From Mine And Instantiate As Inventory Item

    private void RemoveFromMineAndInstantiateInventoryItem(Artifact artifact)
    {
        var cellSize = _mineGenerationVariables.Mine.CellSize;
        var offset = new Vector2(cellSize, cellSize) / 2;
        var artifactPos = new Vector2(artifact.PositionX, artifact.PositionY) * cellSize + offset;
        RemoveArtifactFromMine(artifact);
        InstantiateArtifactAsInventoryItem(artifact, artifactPos);
    }
    
    private void RemoveArtifactFromMine(Artifact artifact)
    {
        var cell = _mineGenerationVariables.Mine.Cells.FirstOrDefault(tempCell =>
            tempCell.HasArtifact && tempCell.ArtifactId == artifact.Id);
        if (cell == null)
        {
            GD.PrintErr("Artifact could not be found inside mine cells");
            return;
        }

        cell.HasArtifact = false;
    }

    private void InstantiateArtifactAsInventoryItem(Artifact artifact, Vector2 pos)
    {
        var rawArtifactFunctional =
            _rawArtifactDto.RawArtifactFunctionals.FirstOrDefault(raw => raw.Id == artifact.RawArtifactId);
        var rawArtifactDescriptive =
            _rawArtifactDto.RawArtifactDescriptives.FirstOrDefault(raw => raw.Id == artifact.RawArtifactId);
        var inventoryItem = new InventoryItem
        {
            Id = artifact.Id,
            Type = "Artifact",
            Category = "",
            Variant = rawArtifactDescriptive!.ArtifactName,
            IsStackable = false,
            Name = rawArtifactDescriptive.ArtifactName,
            PngPath = rawArtifactFunctional!.SmallImageLocation,
            Slot = 0,
            Stack = 1
        };

        var itemPngPath = ReferenceStorage.Instance.ItemDropScenePath;
        var artifactItem =
            SceneInstantiator.InstantiateScene(itemPngPath, _mineGenerationVariables.MineGenView, pos) as ItemDrop;
        if (artifactItem == null)
        {
            GD.PrintErr("Item drop is null");
            return;
        }
        GD.Print("instantiated resource item");
        artifactItem.InventoryItem = inventoryItem;
    }

    #endregion
    
    private void SendArtifactItemToInventory(InventoryItem inventoryItem)
    {
        if (inventoryItem.Type != "Artifact") return;
        
        var inventoryManager = ReferenceStorage.Instance.InventoryManager;
        
        inventoryItem.Slot = inventoryManager.GetNextEmptySlot();
        _inventoryDto.Inventory.OccupiedSlots.Add(inventoryItem.Slot);
        _inventoryDto.Inventory.InventoryItems.Add(inventoryItem);
        MineActions.OnInventoryUpdate?.Invoke();
    }

    #region Exit Tree

    private void UnsubscribeToActions()
    {
        MineActions.OnArtifactSuccessfullyRetrieved -= RemoveFromMineAndInstantiateInventoryItem;
        MineActions.OnCollectItemDrop -= SendArtifactItemToInventory;
    }

    public override void _ExitTree()
    {
        UnsubscribeToActions();
    }

    #endregion
}