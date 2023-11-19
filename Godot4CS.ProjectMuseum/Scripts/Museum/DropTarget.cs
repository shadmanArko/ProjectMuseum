using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class DropTarget : Control
{
    public bool hasEmptySlot = true;
    private PackedScene draggableScene = GD.Load<PackedScene>("res://Scenes/Museum/Museum Ui/Drag And Drop/draggable.tscn");
    // Store the original color for later restoration
    private Color originalColor;

    public override void _Ready()
    {
        originalColor = Modulate; // Save the original color
        MuseumActions.DragStarted += DragStarted;
        MuseumActions.DragEnded += DragEnded;
    }

    private void DragEnded(Draggable obj)
    {
        if (obj.IsInGroup("Draggable"))
        {
            Highlight(false);
        }
    }

    private void DragStarted(Draggable obj)
    {
        if (obj.IsInGroup("Draggable") && hasEmptySlot)
        {
            Highlight(true);
        }
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        var canDrop = ((Node)data).IsInGroup("Draggable");
        GD.Print($"Can drop data has run {Name}");
        // Highlight(canDrop && hasEmptySlot); // Highlight if conditions are met
        return canDrop && hasEmptySlot;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        GD.Print($"Drop data has run");
        hasEmptySlot = false;
        Control draggableCopy = (Control)draggableScene.Instantiate();
        // draggableCopy.GetNode<Draggable>(".").canBeDragged = false;
        draggableCopy.GetNode<Draggable>(".").parentDropTarget = this;
        ((Node)data).GetNode<Draggable>(".").parentDropTarget.hasEmptySlot = true;
        ((Node)data).QueueFree();
        GetNode<Control>(".").AddChild(draggableCopy);
        Highlight(false); // Reset highlight after dropping
    }

    private void Highlight(bool highlight)
    {
        // Set the color to the original or a highlighted color based on the 'highlight' parameter
        Modulate = highlight ? Colors.Green : originalColor;
    }
}