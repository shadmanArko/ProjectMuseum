using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.ArtifactStorage;

public partial class ArtifactFromMineToInventory : Node2D
{
    // private HttpRequest _addArtifactToInventoryHttpRequest;
    private InventoryDTO _inventoryDto;
    private MineGenerationVariables _mineGenerationVariables;
    private RawArtifactDTO _rawArtifactDto;
    
    public override void _Ready()
    {
        SubscribeToActions();
        // CreateHttpRequest();
        InitializeDiReference();
    }

    private void InitializeDiReference()
    {
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
        _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
        _rawArtifactDto = ServiceRegistry.Resolve<RawArtifactDTO>();
    }
    
    // private void CreateHttpRequest()
    // {
    //     _addArtifactToInventoryHttpRequest = new HttpRequest();
    //     AddChild(_addArtifactToInventoryHttpRequest);
    //     _addArtifactToInventoryHttpRequest.RequestCompleted += OnAddArtifactFromMineToInventory;
    // }

    private void SubscribeToActions()
    {
        MineActions.OnArtifactSuccessfullyRetrieved += SendArtifactFromMineToInventory;
    }

    // private void AddArtifactFromMineToInventory(Artifact artifact)
    // {
    //     string[] headers = { "Content-Type: application/json"};
    //     var body = JsonConvert.SerializeObject(artifact);
    //
    //     _addArtifactToInventoryHttpRequest.Request(ApiAddress.MineApiPath+"SendArtifactToInventory", headers,
    //         HttpClient.Method.Post, body);
    //     
    //     GD.Print($"HTTP REQUEST FOR ADDING ARTIFACT TO INVENTORY (3)");
    // }
    //
    // private void OnAddArtifactFromMineToInventory(long result, long responseCode, string[] headers, byte[] body)
    // {
    //     GD.Print($"ARTIFACT SUCCESSFULLY ADDED TO INVENTORY!!! (4)");
    // }
    
    private void SendArtifactFromMineToInventory(Artifact artifact)
    {
        var inventoryManager = ReferenceStorage.Instance.InventoryManager;
        if (!inventoryManager.HasFreeSlot())
        {
            ReferenceStorage.Instance.MinePopUp.ShowPopUp("No empty slots in inventory");
            return;
        }
        
        RemoveArtifactFromMine(artifact);
        var inventoryItem = AddArtifactToInventory(artifact);
        GD.Print($"artifact added to inventory {inventoryItem.Variant}");
        MineActions.OnInventoryUpdate?.Invoke();
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

    private InventoryItem AddArtifactToInventory(Artifact artifact)
    {
        var inventoryManager = ReferenceStorage.Instance.InventoryManager;
        var rawArtifactFunctional =
            _rawArtifactDto.RawArtifactFunctionals.FirstOrDefault(raw => raw.Id == artifact.RawArtifactId);
        var rawArtifactDescriptive =
            _rawArtifactDto.RawArtifactDescriptives.FirstOrDefault(raw => raw.Id == artifact.RawArtifactId);
        var nextEmptySlot = inventoryManager.GetNextEmptySlot();
        var inventoryItem = new InventoryItem
        {
            Id = artifact.Id,
            Type = "Artifact",
            Category = "",
            Variant = rawArtifactDescriptive!.ArtifactName,
            IsStackable = false,
            Name = rawArtifactDescriptive.ArtifactName,
            PngPath = rawArtifactFunctional!.SmallImageLocation,
            Slot = nextEmptySlot,
            Stack = 1
        };

        var inventory = _inventoryDto.Inventory;
        artifact.Slot = nextEmptySlot;
        inventory.OccupiedSlots.Add(nextEmptySlot);
        inventory.InventoryItems.Add(inventoryItem);
        inventory.Artifacts.Add(artifact);

        return inventoryItem;
    }
}