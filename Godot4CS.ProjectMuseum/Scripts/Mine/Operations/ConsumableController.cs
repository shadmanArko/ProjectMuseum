using System.Text;
using System.Text.Json;
// using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Objects.Types.Consumable;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Operations;

public partial class ConsumableController : Node2D
{
    private HttpRequest _sendConsumableFromInventoryHttpRequest;

    private PlayerControllerVariables _playerControllerVariables;
    private MineGenerationVariables _mineGenerationVariables;
    
    private InventoryItem _inventoryItem;
    
    #region Initializers

    private void InitializeDiInstaller()
    {
        CreateHttpRequests();
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineGenerationVariables = ServiceRegistry.Resolve<MineGenerationVariables>();
    }

    private void CreateHttpRequests()
    {
        _sendConsumableFromInventoryHttpRequest = new HttpRequest();
        AddChild(_sendConsumableFromInventoryHttpRequest);
        _sendConsumableFromInventoryHttpRequest.RequestCompleted +=
            OnSendConsumableFromInventoryHttpRequestComplete;
    }

    #region Subscribe and Unsubscribe To Actions

    private void SubscribeToActions()
    {
        MineActions.OnLeftMouseClickActionEnded += ConsumeConsumable;
        MineActions.OnRightMouseClickActionEnded += DestroyConsumableAndDeselect;
    }
    
    private void UnsubscribeToActions()
    {
        MineActions.OnLeftMouseClickActionEnded -= ConsumeConsumable;
        MineActions.OnRightMouseClickActionEnded -= DestroyConsumableAndDeselect;
    }

    #endregion

    public override void _EnterTree()
    {
        InitializeDiInstaller();
    }

    public override void _Ready()
    {
        
    }

    #endregion

    #region Select and Deselect

    public void OnSelectConsumableFromInventory(InventoryItem inventoryItem)
    {
        _inventoryItem = inventoryItem;
        SubscribeToActions();
    }
    
    private void OnDeselectConsumableFromInventory()
    {
        UnsubscribeToActions();
        MineActions.OnInventoryUpdate?.Invoke();
    }
    

    #endregion
    
    private void ConsumeConsumable()
    {
        SendConsumableFromInventory(_inventoryItem);
    }
    private void SendConsumableFromInventory(InventoryItem inventoryItem)
    {
        string[] headers = { "Content-Type: application/json"};
        GD.Print($"inventory item: {inventoryItem.Variant}, stack:{inventoryItem.Stack}, slot:{inventoryItem.Slot}, id:{inventoryItem.Id}");
        
        var url = ApiAddress.PlayerApiPath + "SendConsumableFromInventoryToMine/" + inventoryItem.Id;
        _sendConsumableFromInventoryHttpRequest.CancelRequest();
        _sendConsumableFromInventoryHttpRequest.Request(url, headers, HttpClient.Method.Post);
    }
    
    private void OnSendConsumableFromInventoryHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        GD.Print($"json string: {jsonStr}");
        var consumable = JsonSerializer.Deserialize<Consumable>(jsonStr);
        GD.Print($"consumable: {consumable}");
        var scenePath = consumable.ScenePath;
        var consumableItem = SceneInstantiator.InstantiateScene(scenePath, _mineGenerationVariables.MineGenView, Vector2.Zero) as IConsumable;
        consumableItem?.ApplyStatEffect();
        OnDeselectConsumableFromInventory();
    }
    
    private void DestroyConsumableAndDeselect()
    {
        OnDeselectConsumableFromInventory();
    }
}