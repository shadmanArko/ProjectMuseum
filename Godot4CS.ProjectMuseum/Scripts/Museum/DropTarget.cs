using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class DropTarget : ColorRect
{
    private bool hasEmptySlot = true;

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
        Highlight(canDrop && hasEmptySlot); // Highlight if conditions are met
        return canDrop && hasEmptySlot;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        GD.Print($"Drop data has run");
        hasEmptySlot = false;
        Highlight(false); // Reset highlight after dropping
    }

    private void Highlight(bool highlight)
    {
        // Set the color to the original or a highlighted color based on the 'highlight' parameter
        Modulate = highlight ? Colors.Green : originalColor;
    }
}