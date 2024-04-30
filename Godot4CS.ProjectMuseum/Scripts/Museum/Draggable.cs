using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
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
	[Export] private Label _nameOfDraggable;
	[Export] private TextureRect _artifactIcon;
	[Export] private PackedScene _draggablePreview;
	[Export] private Control _artifactTagParent1;
	[Export] private Control _artifactTagParent2;
	[Export] private PackedScene _artifactTag;
	private static List<RawArtifactDescriptive> _rawArtifactDescriptiveDatas;
	private static List<RawArtifactFunctional> _rawArtifactFunctionalDatas;
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

	public void Initialize(Artifact artifact, List<RawArtifactDescriptive> rawArtifactDescriptives, List<RawArtifactFunctional> rawArtifactFunctionals)
	{
		if (artifact == null)
		{
			//GD.Print("No artifact");
			return;
		}

		_rawArtifactDescriptiveDatas = rawArtifactDescriptives;
		_rawArtifactFunctionalDatas = rawArtifactFunctionals;
		RawArtifactDescriptive rawArtifactDescriptive =
			rawArtifactDescriptives.FirstOrDefault(descriptive => descriptive.Id == artifact.RawArtifactId);
		RawArtifactFunctional rawArtifactFunctional =
			rawArtifactFunctionals.FirstOrDefault(descriptive => descriptive.Id == artifact.RawArtifactId);
		_nameOfDraggable.Text = rawArtifactDescriptive.ArtifactName;
		_artifactIcon.Texture = GD.Load<Texture2D>(rawArtifactFunctional.LargeImageLocation);
		
		InstantiateArtifactTag(rawArtifactFunctional.Era);
		InstantiateArtifactTag(rawArtifactFunctional.Region);
		InstantiateArtifactTag(rawArtifactFunctional.Object);
		InstantiateArtifactTag(rawArtifactFunctional.ObjectSize);
		foreach (var material in rawArtifactFunctional.Materials)
		{
			InstantiateArtifactTag(material);
		}
		Artifact = artifact;
	}

	private int _tagsCount = 0;
	private void InstantiateArtifactTag(string tag)
	{
		_tagsCount++;
		var instance = _artifactTag.Instantiate();
		var parent = _tagsCount < 4 ? _artifactTagParent1 : _artifactTagParent2;
		parent.AddChild(instance);
		instance.GetNode<ExhibitEditorArtifactTag>(".").Initialize(tag);
	}

	private void StartDrag()
	{
		isDragging = true;
		//GD.Print("Drag started");
		MuseumActions.DragStarted?.Invoke(this);
		// Additional logic to initialize drag, if needed
	}

	public void ResetDraggableOnGettingBackToParent()
	{
		// CustomMinimumSize = new Vector2(387, 100);
	}
	private void EndDrag()
	{
		if (isDragging)
		{
			//GD.Print("Drag ended");
			MuseumActions.DragEnded?.Invoke(this);
			// Additional logic to finalize drag, if needed
		}
		isDragging = false;
	}
	public override Variant _GetDragData(Vector2 atPosition)
	{
		SlotAtTheStartOfDrag = parentDropTarget.SlotNumber;
		//GD.Print($"get_drag_data has run");
		if (!droppedOnTarget)
		{
			var draggablePreviewInstance = _draggablePreview.Instantiate();
			draggablePreviewInstance.GetNode<DraggableNewPreview>(".").Initialize(Artifact,
				_rawArtifactDescriptiveDatas, _rawArtifactFunctionalDatas);
			SetDragPreview((Control)draggablePreviewInstance);
			
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
