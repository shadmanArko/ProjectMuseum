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
    [Export] private float _speed;

    public InventoryItem InventoryItem
    {
        get => _inventoryItem;
        set
        {
            _inventoryItem = value;
            _itemSprite.Texture = SetSprite(value.PngPath);
        }
    }

    #region Initializers

    public override void _Ready()
    {
        InitializeDiInstaller();
        SetPhysicsProcess(false);
    }

    private void InitializeDiInstaller()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
    }

    #endregion

    public override void _PhysicsProcess(double delta)
    {
        var playerPos = _playerControllerVariables.Position;
        LinearVelocity = (playerPos - Position) * _speed * (float)delta;
    }

    #region Utilities

    private Texture2D SetSprite(string spritePath)
    {
        var texture2D = GD.Load<Texture2D>(spritePath);
        return texture2D;
    }

    #endregion

    public override void _ExitTree()
    {
        SetPhysicsProcess(false);
    }
}