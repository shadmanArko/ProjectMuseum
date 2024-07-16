using Godot;
using System;
using System.Collections.Generic;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.Models;

public partial class DropTarget : Control
{
    [Export] private bool _parentTarget;
    public bool hasEmptySlot = true;
    public bool canDropItem = true;
    private bool matchsGrid = false;
    private PackedScene draggableScene = GD.Load<PackedScene>("res://Scenes/Museum/Museum Ui/Drag And Drop/draggable.tscn");
    private PackedScene draggableScene100x100 = GD.Load<PackedScene>("res://Scenes/Museum/Museum Ui/Drag And Drop/draggable_100x100.tscn");
    // Store the original color for later restoration
    private Color originalColor;
    [Export] public int SlotNumber = 0;
    [Export] public int GridNumber = 0;
    private List<RawArtifactDescriptive> _rawArtifactDescriptiveDatas;
    private List<RawArtifactFunctional> _rawArtifactFunctionalDatas;
    public override void _Ready()
    {
        originalColor = Modulate; // Save the original color
        MuseumActions.DragStarted += DragStarted;
        MuseumActions.DragEnded += DragEnded;
        MuseumActions.OnRawArtifactDescriptiveDataLoaded += OnRawArtifactDescriptiveDataLoaded;
        MuseumActions.OnRawArtifactFunctionalDataLoaded += OnRawArtifactFunctionalDataLoaded;
        MuseumActions.OnMakeGridSlotEligible += OnMakeGridSlotEligible;
        MuseumActions.OnMakeGridSlotDisable += OnMakeGridSlotDisable;
    }

    private void OnMakeGridSlotDisable(int grid, int slot)
    {
        if (_parentTarget) return;
        if (grid == GridNumber && slot == SlotNumber)
        {
            GD.Print($"Grid {grid}, slot {slot} disable");
            hasEmptySlot = false;
            SetGridSizeInfo(false);
        }
    }

    private void OnMakeGridSlotEligible(int grid, int slot)
    {
        if (_parentTarget) return;
        if (grid == GridNumber && slot == SlotNumber)
        {
            GD.Print($"Grid {grid}, slot {slot} eligible");
            hasEmptySlot = true;
            SetGridSizeInfo(true);
        }
    }

    private void OnRawArtifactFunctionalDataLoaded(List<RawArtifactFunctional> obj)
    {
        _rawArtifactFunctionalDatas = obj;
    }

    private void OnRawArtifactDescriptiveDataLoaded(List<RawArtifactDescriptive> obj)
    {
        _rawArtifactDescriptiveDatas = obj;
    }

    public void Initialize(int slotNumber, int gridNumber, List<RawArtifactDescriptive> rawArtifactDescriptives, List<RawArtifactFunctional> rawArtifactFunctionals)
    {
        GD.Print($"initialized grid {gridNumber}, slot {slotNumber}");
        SlotNumber = slotNumber;
        GridNumber = gridNumber;
        _rawArtifactDescriptiveDatas = rawArtifactDescriptives;
        _rawArtifactFunctionalDatas = rawArtifactFunctionals;
    }
    private void DragEnded(Draggable obj)
    {
        if (obj.IsInGroup("Draggable"))
        {
            Modulate = originalColor;
        }
    }

    private void DragStarted(Draggable obj)
    {
        if (obj.IsInGroup("Draggable") && hasEmptySlot)
        {
            // Highlight(true);
        }
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        var canDrop = ((Node)data).IsInGroup("Draggable");
        //GD.Print($"Can drop data has run {Name}");
        // Highlight(canDrop && hasEmptySlot); // Highlight if conditions are met
        return (canDrop && hasEmptySlot) || _parentTarget;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        //GD.Print($"Drop data has run");
        hasEmptySlot = false;
        Control draggableCopy = _parentTarget? (Control)draggableScene.Instantiate() : (Control)draggableScene100x100.Instantiate();
        // draggableCopy.GetNode<Draggable>(".").canBeDragged = false;
        draggableCopy.GetNode<Draggable>(".").parentDropTarget = this;
        draggableCopy.GetNode<Draggable>(".").Initialize(((Node)data).GetNode<Draggable>(".").Artifact, _rawArtifactDescriptiveDatas, _rawArtifactFunctionalDatas);
        ((Node)data).GetNode<Draggable>(".").parentDropTarget.hasEmptySlot = true;
        
        GetNode<Control>(".").AddChild(draggableCopy);
        
        if (_parentTarget)
        {
            draggableCopy.GetNode<Draggable>(".").ResetDraggableOnGettingBackToParent();
            MuseumActions.ArtifactRemovedFromSlot?.Invoke(((Node)data).GetNode<Draggable>(".").Artifact, ((Node)data).GetNode<Draggable>(".").SlotAtTheStartOfDrag, ((Node)data).GetNode<Draggable>(".").GridAtTheStartOfDrag);
        }
        else
        {
            var artifact = ((Node)data).GetNode<Draggable>(".").Artifact;
            var artifactSize = ((Node)data).GetNode<Draggable>(".").ArtifactSize;
            MuseumActions.ArtifactRemovedFromSlot?.Invoke(artifact,
                ((Node)data).GetNode<Draggable>(".").SlotAtTheStartOfDrag, ((Node)data).GetNode<Draggable>(".").GridAtTheStartOfDrag);
            MuseumActions.ArtifactDroppedOnSlot?.Invoke(artifact, artifactSize, SlotNumber, GridNumber);
            MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("PlaceArtifactOnDisplaySlot");

        }
        Highlight(false); 
        ((Node)data).QueueFree();// Reset highlight after dropping
    }

    public void Highlight(bool highlight)
    {
        // Set the color to the original or a highlighted color based on the 'highlight' parameter
        Modulate = highlight ? Colors.Green : Colors.Red;
    }

    public void SetGridSizeInfo(bool value)
    {
        matchsGrid = value;
        canDropItem = value;
        Highlight(matchsGrid);
    }

    public override void _ExitTree()
    {
        MuseumActions.DragStarted -= DragStarted;
        MuseumActions.DragEnded -= DragEnded;
        MuseumActions.OnRawArtifactDescriptiveDataLoaded -= OnRawArtifactDescriptiveDataLoaded;
        MuseumActions.OnRawArtifactFunctionalDataLoaded -= OnRawArtifactFunctionalDataLoaded;
        MuseumActions.OnMakeGridSlotEligible -= OnMakeGridSlotEligible;
        MuseumActions.OnMakeGridSlotDisable -= OnMakeGridSlotDisable;
    }
}