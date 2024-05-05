using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Scripts.Test;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.InventorySystem;

public partial class InventoryManager : Node2D
{
    private Inventory _inventory;
    
    private HttpRequest _getInventoryHttpRequest;
    
    [Export] private MouseFollowingSprite _mouseFollowingSprite;
    [Export] private InventorySlot _invSlot;


    #region Initializers

    public override void _EnterTree()
    {
        CreateHttpRequest();
    }

    public override async void _Ready()
    {
        InitializeDiReferences();
        await Task.Delay(5000);
        GetInventory();
    }
    
    private void InitializeDiReferences()
    {
        _inventory = ServiceRegistry.Resolve<Inventory>();
    }

    private void CreateHttpRequest()
    {
        _getInventoryHttpRequest = new HttpRequest();
        AddChild(_getInventoryHttpRequest);
        _getInventoryHttpRequest.RequestCompleted += OnGetInventoryHttpRequestCompleted;
    }

    #endregion

    #region Get Inventory Http Request 

    private void GetInventory()
    {
        var url = ApiAddress.PlayerApiPath+"GetInventory";
        _getInventoryHttpRequest.CancelRequest();
        _getInventoryHttpRequest.Request(url);
    }
	   
    private void OnGetInventoryHttpRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        var jsonStr = Encoding.UTF8.GetString(body);
        _inventory = JsonSerializer.Deserialize<Inventory>(jsonStr);
        _invSlot.SetInventoryItemToSlot(_inventory.InventoryItems[0]);
    }

    #endregion
    
    public void SetItemFromInventorySlotToMouseCursor(InventoryItem inventoryItem)
    {
        _mouseFollowingSprite.ShowMouseFollowSprite(inventoryItem);
        var isRemoved = _inventory.InventoryItems.Remove(inventoryItem);
        
    }
    
}