using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects;

public partial class ItemDrop : RigidBody2D
{
    private PlayerControllerVariables _playerControllerVariables;
    
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

    public InventoryItem GetItem() => _inventoryItem;

    private Texture2D SetSprite(string spritePath)
    {
        var texture2D = GD.Load<Texture2D>(spritePath);
        return texture2D;
    }
}