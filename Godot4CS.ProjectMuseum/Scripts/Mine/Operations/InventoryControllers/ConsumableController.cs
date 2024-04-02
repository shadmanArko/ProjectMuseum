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
        MineActions.OnLeftMouseClickActionEnded += CheckForActionEligibility;
        MineActions.DeselectAllInventoryControllers += DeactivateController;
    }
    
    private void UnsubscribeToActions()
    {
        MineActions.OnLeftMouseClickActionEnded -= CheckForActionEligibility;
        MineActions.DeselectAllInventoryControllers -= DeactivateController;
    }

    #endregion

    #region Activate Deactivate Controller

    public override void ActivateController(InventoryItem inventoryItem)
    {
        IsControllerActivated = true;
        _inventoryItem = inventoryItem;
        SubscribeToActions();
        GD.Print("Health potion controller activated");
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
        var eligible = _playerControllerVariables.PlayerHealth < 200;
        GD.Print($"health potion consumption eligibility is: {eligible}");
        if (!eligible)
        {
            await ReferenceStorage.Instance.MinePopUp.ShowPopUp("Player already in full health");
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
    
    private async void OnConsumeHealthPotionFromInventoryHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        GD.Print($"json string: {jsonStr}");
        var consumable = JsonSerializer.Deserialize<Consumable>(jsonStr);
        GD.Print($"consumable: {consumable}");

        foreach (var statEffect in consumable.ConsumableStatEffects)
        {
            switch (statEffect.StatName)
            {
                case "Health":
                    HealthSystem.EffectPlayerHealth(statEffect, _playerControllerVariables);
                    GD.Print("health effect ");
                    break;
            }
        }
        
        MineActions.OnInventoryUpdate?.Invoke();
        await ReferenceStorage.Instance.MinePopUp.ShowPopUp("Health increased by 50 Hp");
    }

    #endregion
}