using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations.InventoryControllers;

public partial class ConsumableController : InventoryController
{
    private HttpRequest _sendConsumableFromInventoryHttpRequest;

    private PlayerControllerVariables _playerControllerVariables;

    private InventoryItem _inventoryItem;
    
    #region Initializers

    private void InitializeDiInstaller()
    {
        CreateHttpRequests();
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void CreateHttpRequests()
    {
        _sendConsumableFromInventoryHttpRequest = new HttpRequest();
        AddChild(_sendConsumableFromInventoryHttpRequest);
        _sendConsumableFromInventoryHttpRequest.RequestCompleted +=
            OnConsumeHealthPotionFromInventoryHttpRequestComplete;
    }

    public override void _EnterTree()
    {
        InitializeDiInstaller();
    }

    public override void _Ready()
    {
        
    }

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

    private async void CheckForActionEligibility()
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
                "Health" => "Player is already in full health",
                "Energy" => "Player is already in full energy",
                _ => "An unknown error occured"
            };
            ReferenceStorage.Instance.MinePopUp.ShowPopUp(message);
            return;
        }
        
        ConsumeHealthPotionFromInventory(_inventoryItem);
    }

    #region Send Consumable From Inventory

    private void ConsumeHealthPotionFromInventory(InventoryItem inventoryItem)
    {
        string[] headers = { "Content-Type: application/json"};
        GD.Print($"inventory item: {inventoryItem.Variant}, stack:{inventoryItem.Stack}, slot:{inventoryItem.Slot}, id:{inventoryItem.Id}");
        
        var url = ApiAddress.PlayerApiPath + "SendConsumableFromInventoryToMine/" + inventoryItem.Id;
        _sendConsumableFromInventoryHttpRequest.CancelRequest();
        _sendConsumableFromInventoryHttpRequest.Request(url, headers, HttpClient.Method.Post);
    }
    
    private void OnConsumeHealthPotionFromInventoryHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        GD.Print($"json string: {jsonStr}");
        var consumable = JsonSerializer.Deserialize<Consumable>(jsonStr);
        GD.Print($"consumable: {consumable}");

        foreach (var statEffect in consumable.ConsumableStatEffects)
        {
            switch (consumable.Category)
            {
                case "Health":
                    HealthSystem.EffectPlayerHealth(statEffect, _playerControllerVariables);
                    ReferenceStorage.Instance.MinePopUp.ShowPopUp("Health increased");
                    break;
                case "Energy":
                    EnergySystem.EffectPlayerEnergy(statEffect, _playerControllerVariables);
                    ReferenceStorage.Instance.MinePopUp.ShowPopUp("Energy increased");
                    break;
            }
        }
        
        MineActions.OnInventoryUpdate?.Invoke();
    }

    #endregion
}