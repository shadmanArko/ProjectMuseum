using Godot;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;

public partial class Museum : Node2D
{

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GameManager.TileMap = GetNode<TileMap>("TileMap");
    }
}