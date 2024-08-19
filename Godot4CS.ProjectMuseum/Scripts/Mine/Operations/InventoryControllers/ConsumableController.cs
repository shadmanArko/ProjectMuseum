using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations.InventoryControllers;

public partial class ConsumableController : InventoryController
{
    private HttpRequest _getAllConsumableDtoHttpRequest;

    private PlayerControllerVariables _playerControllerVariables;
    private InventoryDTO _inventoryDto;
    private ConsumableDTO _consumableDto;

    private InventoryItem _inventoryItem;
    
    #region Initializers

    private void CreateHttpRequests()
    {
        _getAllConsumableDtoHttpRequest = new HttpRequest();
        AddChild(_getAllConsumableDtoHttpRequest);
        _getAllConsumableDtoHttpRequest.RequestCompleted += OnGetAllConsumableDataComplete;
    }

    private void InitializeDiInstaller()
    {
        // CreateHttpRequests();
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        ServiceRegistry.Resolve<MineGenerationVariables>();
        _inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
        _consumableDto = ServiceRegistry.Resolve<ConsumableDTO>();
    }
    
    public override void _Ready()
    {
        InitializeDiInstaller();
        // GetAllConsumableData();
    }

    #region Populate Consumable DTO

    private void GetAllConsumableData()
    {
        // var url = ApiAddress.MineApiPath + "GetAllConsumables";
        // _getAllConsumableDtoHttpRequest.CancelRequest();
        // _getAllConsumableDtoHttpRequest.Request(url);
    }
    
    private void OnGetAllConsumableDataComplete(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        var consumables = JsonSerializer.Deserialize<List<Consumable>>(jsonStr);
        
        if (consumables == null)
        {
            GD.PrintErr("Consumables is null in Consumable DTO");
            return;
        }
        
        // _consumableDto.Consumables = consumables;
    }

    #endregion

    #endregion
    
    #region Subscribe and Unsubscribe To Actions

    private void SubscribeToActions()
    {
        MineActions.OnLeftMouseClickAction += CheckForActionEligibility;
        MineActions.DeselectAllInventoryControllers += DeactivateController;
    }
    
    private void UnsubscribeToActions()
    {
        MineActions.OnLeftMouseClickAction -= CheckForActionEligibility;
        MineActions.DeselectAllInventoryControllers -= DeactivateController;
    }

    #endregion

    #region Activate Deactivate Controller

    public override void ActivateController(InventoryItem inventoryItem)
    {
        IsControllerActivated = true;
        _inventoryItem = inventoryItem;
        SubscribeToActions();
    }
    
    public override void DeactivateController()
    {
        if(!IsControllerActivated) return;
        IsControllerActivated = false;
        UnsubscribeToActions();
        MineActions.OnInventoryUpdate?.Invoke();
        GD.Print("Health potion controller deactivated");
    }
    
    #endregion

    private void CheckForActionEligibility()
    {
        var eligible = _inventoryItem.Category switch
        {
            "Health" => _playerControllerVariables.PlayerHealth < 200,
            "Energy" => _playerControllerVariables.PlayerEnergy < 200,
            _ => false
        };
        
        if (!eligible)
        {
            var message = _inventoryItem.Category switch
            {
                "Health" => "Already at full health.",
                "Energy" => "Already at full energy.",
                _ => "An unknown error occured"
            };
            ReferenceStorage.Instance.LogMessageController.ShowLogMessage(message);
            return;
        }
        
        ConsumeHealthPotionFromInventory(_inventoryItem);
    }

    #region Send Consumable From Inventory

    private void ConsumeHealthPotionFromInventory(InventoryItem inventoryItem)
    {
        RemoveInventoryItem(inventoryItem.Id);
        var consumable = GetConsumableByVariant(inventoryItem);
        foreach (var statEffect in consumable.ConsumableStatEffects)
        {
            switch (consumable.Category)
            {
                case "Health":
                    HealthSystem.EffectPlayerHealth(statEffect, _playerControllerVariables);
                    ReferenceStorage.Instance.LogMessageController.ShowLogMessage("Health increased");
                    break;
                case "Energy":
                    EnergySystem.EffectPlayerEnergy(statEffect, _playerControllerVariables);
                    ReferenceStorage.Instance.LogMessageController.ShowLogMessage("Energy increased");
                    break;
            }
        }
        
        MineActions.OnInventoryUpdate?.Invoke();
    }

    private void RemoveInventoryItem(string inventoryItemId)
    {
        var inventoryItemToRemove =
            _inventoryDto.Inventory.InventoryItems.FirstOrDefault(item1 => item1.Id == inventoryItemId);
        if (inventoryItemToRemove == null)
        {
            GD.PrintErr("Consumable could not be found in inventory");
            return;
        }
        
        if (inventoryItemToRemove!.IsStackable && inventoryItemToRemove.Stack > 1)
            inventoryItemToRemove.Stack--;
        else 
            _inventoryDto.Inventory.InventoryItems.Remove(inventoryItemToRemove);
    }
    
    private Consumable GetConsumableByVariant(InventoryItem inventoryItem)
    {
        var consumable = _consumableDto.Consumables.FirstOrDefault(temp => temp.Variant == inventoryItem.Variant);
        if (consumable == null)
        {
            GD.PrintErr("Consumable could not be found in consumable database");
            return null;
        }

        return consumable;
    }
    
    #endregion
}