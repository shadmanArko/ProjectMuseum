using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Museum;

public partial class ExhibitController : Node2D
{
    [Export] private ExhibitEditorUi _exhibitEditorUi;
    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_right_click"))
        {
            CheckClick(GetGlobalMousePosition());
        }
    }

    private void CheckClick(Vector2 globalMousePosition)
    {
        // Get the nodes that are clickable (you can customize this based on your scene structure)
        var clickableNodes = GetTree().GetNodesInGroup("Exhibit");
        GD.Print("Checking click");
        foreach (Node clickableNode in clickableNodes)
        {
            if (clickableNode is Sprite2D sprite)
            {
                // Check if the mouse is within the bounding box of the sprite
                Rect2 rect = new Rect2(sprite.GlobalPosition, sprite.GetRect().Size);

                if (rect.HasPoint(globalMousePosition))
                {
                    // The clickable node is clicked
                    OpenUiForNode(clickableNode);
                    return; // Stop checking further nodes
                }
            }
        }
    }

    private void OpenUiForNode(Node node)
    {
        // Perform actions specific to the clicked node
        GD.Print("Clicked on: " + node.Name);
        // Add your UI opening logic here
    }
}