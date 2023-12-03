using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.Models;

public partial class Draggable : ColorRect
{
	private bool droppedOnTarget = false;
	private bool isDragging = false;
	public bool canBeDragged = true;
	public DropTarget parentDropTarget;
	private Vector2 _customMinimumSize;
	public int SlotAtTheStartOfDrag;
	public Artifact Artifact;
	[Export] private RichTextLabel _nameOfDraggable;
	[Export] private PackedScene _draggablePreview;
	public override void _Ready()
	{
		AddToGroup("Draggable");
		_customMinimumSize = CustomMinimumSize;
		parentDropTarget = GetParent<DropTarget>();
		SlotAtTheStartOfDrag = parentDropTarget.SlotNumber;
	}
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (Input.IsActionJustPressed("ui_left_click"))
			{
				if (GetRect().HasPoint(GetLocalMousePosition()))
				{
					// Check if the left mouse button is held down after a click
					if (Input.IsActionPressed("ui_left_click"))
					{
						StartDrag();
					}
				} 
			}
			else if (Input.IsActionJustReleased("ui_left_click"))
			{
				EndDrag();
			}
			
		}
	}

	public void Initialize(Artifact artifact)
	{
		if (artifact == null)
		{
			GD.Print("No artifact");
			return;
		}
		_nameOfDraggable.Text = artifact.RawArtifactId;
		Artifact = artifact;
	}
	private void StartDrag()
	{
		isDragging = true;
		GD.Print("Drag started");
		MuseumActions.DragStarted?.Invoke(this);
		// Additional logic to initialize drag, if needed
	}

	public void ResetDraggableOnGettingBackToParent()
	{
		CustomMinimumSize = new Vector2(200, 50);
	}
	private void EndDrag()
	{
		if (isDragging)
		{
			GD.Print("Drag ended");
			MuseumActions.DragEnded?.Invoke(this);
			// Additional logic to finalize drag, if needed
		}
		isDragging = false;
	}
	public override Variant _GetDragData(Vector2 atPosition)
	{
		
		GD.Print($"get_drag_data has run");
		if (!droppedOnTarget)
		{
			SetDragPreview((Control)_draggablePreview.Instantiate());
			
			return this;
		}

		return this;
	}
	

	private Control _GetPreviewControl()
	{
		/*
		The preview control must not be in the scene tree. You should not free the control, and
		you should not keep a reference to the control beyond the duration of the drag.
		It will be deleted automatically after the drag has ended.
		*/
		var preview = new ColorRect();
		preview.Size = _customMinimumSize;
		var previewColor = Modulate;
		previewColor.A = 0.5f;
		preview.Modulate = previewColor;
		var rotationQuat = new Quaternion(Vector3.Forward, 30f);
		preview.RotationDegrees = rotationQuat.GetAngle();
		return preview;
	}

}
