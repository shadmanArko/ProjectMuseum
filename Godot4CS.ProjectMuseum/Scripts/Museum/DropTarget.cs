using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.Models;

public partial class DropTarget : Control
{
    [Export] private bool _parentTarget;
    public bool hasEmptySlot = true;
    private PackedScene draggableScene = GD.Load<PackedScene>("res://Scenes/Museum/Museum Ui/Drag And Drop/draggable.tscn");
    // Store the original color for later restoration
    private Color originalColor;
    public int SlotNumber = 0;
    public override void _Ready()
    {
        originalColor = Modulate; // Save the original color
        MuseumActions.DragStarted += DragStarted;
        MuseumActions.DragEnded += DragEnded;
    }

    public void Initialize(int slotNumber)
    {
        SlotNumber = slotNumber;
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
        return (canDrop && hasEmptySlot) || _parentTarget;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        GD.Print($"Drop data has run");
        hasEmptySlot = false;
        Control draggableCopy = (Control)draggableScene.Instantiate();
        // draggableCopy.GetNode<Draggable>(".").canBeDragged = false;
        draggableCopy.GetNode<Draggable>(".").parentDropTarget = this;
        draggableCopy.GetNode<Draggable>(".").Initialize(((Node)data).GetNode<Draggable>(".").Artifact);
        ((Node)data).GetNode<Draggable>(".").parentDropTarget.hasEmptySlot = true;
        
        GetNode<Control>(".").AddChild(draggableCopy);
        
        if (_parentTarget)
        {
            draggableCopy.GetNode<Draggable>(".").ResetDraggableOnGettingBackToParent();
            MuseumActions.ArtifactRemovedFromSlot?.Invoke(((Node)data).GetNode<Draggable>(".").Artifact, ((Node)data).GetNode<Draggable>(".").SlotAtTheStartOfDrag);
        }
        else
        {
            MuseumActions.ArtifactRemovedFromSlot?.Invoke(((Node)data).GetNode<Draggable>(".").Artifact,
                ((Node)data).GetNode<Draggable>(".").SlotAtTheStartOfDrag);
            MuseumActions.ArtifactDroppedOnSlot?.Invoke(((Node)data).GetNode<Draggable>(".").Artifact, SlotNumber);
        }
        Highlight(false); 
        ((Node)data).QueueFree();// Reset highlight after dropping
    }

    private void Highlight(bool highlight)
    {
        // Set the color to the original or a highlighted color based on the 'highlight' parameter
        Modulate = highlight ? Colors.Green : originalColor;
    }

    public override void _ExitTree()
    {
        MuseumActions.DragStarted -= DragStarted;
        MuseumActions.DragEnded -= DragEnded;
    }
}