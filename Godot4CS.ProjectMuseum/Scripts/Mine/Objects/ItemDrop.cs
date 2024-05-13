using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects;

public partial class ItemDrop : Node2D
{
    private PlayerControllerVariables _playerControllerVariables;
    
    [Export] private Area2D _area2D;
    [Export] private Sprite2D _itemSprite;

    private InventoryItem _inventoryItem;

    public override void _Ready()
    {
        
    }

    private void InitializeDiReference()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
    }

    public void SetItem(InventoryItem item)
    {
        _inventoryItem = item;
        _itemSprite.Texture = SetSprite(item.PngPath);
    }

    private Texture2D SetSprite(string spritePath)
    {
        var texture2D = GD.Load<Texture2D>(spritePath);
        return texture2D;
    }

    private void OnBodyEnter(Node body)
    {
        if (body == _playerControllerVariables.Player)
        {
            
        }
    }

    private void OnBodyExit(Node body)
    {
        
    }

}