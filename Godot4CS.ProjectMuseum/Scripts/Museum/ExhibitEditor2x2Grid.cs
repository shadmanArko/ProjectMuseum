using Godot;
using System;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.Models;

public partial class ExhibitEditor2x2Grid : GridContainer
{
	[Export] private Array<DropTarget> _dropTargets;
	[Export] private int _gridNumber;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.DragStarted += DragStarted;
		MuseumActions.ArtifactDroppedOnSlot += ArtifactDroppedOnSlot;
	}

	private void ArtifactDroppedOnSlot(Artifact artifact, string size, int slot, int grid)
	{
		if ( _gridNumber != grid)
		{
			return;
		}
		GD.Print("Drop targets count " +_dropTargets.Count);
		if (size == "Medium" )
		{
			foreach (var dropTarget in _dropTargets)
			{
				dropTarget.hasEmptySlot = false;
				_dropTargets[0].hasEmptySlot = false;
				_dropTargets[2].hasEmptySlot = false;
				_dropTargets[1].hasEmptySlot = false;
				_dropTargets[3].hasEmptySlot = false;
			}
		}
		if (size == "Small" )
		{
			if (slot == 0 || slot == 2)
			{
				_dropTargets[0].hasEmptySlot = false;
				_dropTargets[2].hasEmptySlot = false;
			}else if (slot ==1 || slot == 3)
			{
				_dropTargets[1].hasEmptySlot = false;
				_dropTargets[3].hasEmptySlot = false;
			}
		}
		if (size == "Tiny" )
		{
			_dropTargets[slot].hasEmptySlot = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void DragEnded(Draggable obj)
	{
		//
	}

	private void DragStarted(Draggable obj)
	{
		GD.Print("Drag Started Called from grid");
		if (obj.IsInGroup("Draggable") )
		{
			if (obj.ArtifactSize == "Tiny")
			{
				foreach (var dropTarget in _dropTargets)
				{
					dropTarget.SetGridSizeInfo(dropTarget.hasEmptySlot);
				}
			}
			if (obj.ArtifactSize == "Small")
			{
				_dropTargets[0].SetGridSizeInfo(_dropTargets[0].hasEmptySlot && _dropTargets[2].hasEmptySlot);
				_dropTargets[2].SetGridSizeInfo(_dropTargets[0].hasEmptySlot && _dropTargets[2].hasEmptySlot);
				_dropTargets[1].SetGridSizeInfo(_dropTargets[1].hasEmptySlot && _dropTargets[3].hasEmptySlot);
				_dropTargets[3].SetGridSizeInfo(_dropTargets[1].hasEmptySlot && _dropTargets[3].hasEmptySlot);
			}
			if (obj.ArtifactSize == "Medium")
			{
				var wholeGridEligible = true;
				foreach (var dropTarget in _dropTargets)
				{
					if (!dropTarget.hasEmptySlot)
					{
						wholeGridEligible = false;
					} 
				}

				foreach (var dropTarget in _dropTargets)
				{
					dropTarget.SetGridSizeInfo(wholeGridEligible);
				}
			}
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.DragStarted -= DragStarted;
	}
}
