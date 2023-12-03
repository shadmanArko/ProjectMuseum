using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

public partial class ExhibitEditorUi : Control
{
	[Export] private Button _exitButton;
	[Export] private PackedScene _draggable;
	[Export] private PackedScene _dropTarget;
	[Export] private Control _draggablesParent;
	[Export] private Control _dropTargetsParent;
	private Item _selectedItem;
	private HttpRequest _httpRequestForGettingExhibitsInStore;
	private HttpRequest _httpRequestForGettingExhibitsInDisplay;

	private Exhibit _selectedExhibit;
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_httpRequestForGettingExhibitsInStore = new HttpRequest();
		_httpRequestForGettingExhibitsInDisplay = new HttpRequest();
		AddChild(_httpRequestForGettingExhibitsInStore);
		AddChild(_httpRequestForGettingExhibitsInDisplay);
		_httpRequestForGettingExhibitsInStore.RequestCompleted += HttpRequestForGettingExhibitsInStoreOnRequestCompleted;
		_httpRequestForGettingExhibitsInDisplay.RequestCompleted += HttpRequestForGettingExhibitsInDisplayOnRequestCompleted;
		MuseumActions.ArtifactDroppedOnSlot += ArtifactDroppedOnSlot;
		MuseumActions.ArtifactRemovedFromSlot += ArtifactRemovedFromSlot;
		_exitButton.Pressed += ExitButtonOnPressed;
	}

	private void ArtifactRemovedFromSlot(Artifact artifact, int slotNumber)
	{
		MuseumActions.ArtifactRemovedFromExhibitSlot?.Invoke(artifact, _selectedItem, slotNumber);
	}

	private void HttpRequestForGettingExhibitsInDisplayOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var artifacts = JsonSerializer.Deserialize<List<Artifact>>(jsonStr);
		foreach (var artifact in artifacts)
		{
			if (artifact.Id == _selectedExhibit.ExhibitArtifactSlot1)
			{
				var instance = _draggable.Instantiate();
				_dropTargetsParent.GetChildren()[0].AddChild(instance);
				instance.GetNode<Draggable>(".").Initialize(artifact);
			}else if (artifact.Id == _selectedExhibit.ExhibitArtifactSlot2)
			{
				var instance = _draggable.Instantiate();
				_dropTargetsParent.GetChildren()[1].AddChild(instance);
				instance.GetNode<Draggable>(".").Initialize(artifact);
			}
			
		}
	}

	private void ArtifactDroppedOnSlot(Artifact artifact, int slotNumber)
	{
		MuseumActions.ArtifactDroppedOnExhibitSlot?.Invoke(artifact, _selectedItem, slotNumber);
	}

	public void ReInitialize(Item item, Exhibit exhibit)
	{
		_selectedExhibit = exhibit;
		_httpRequestForGettingExhibitsInStore.Request(ApiAddress.MuseumApiPath + "GetAllArtifactsInStorage");
		DeleteChild(_dropTargetsParent);
		_selectedItem = item;
		var numberOfSlots = item.numberOfTilesItTakes < 4 ? 1 : 2;
		for (int i = 0; i < numberOfSlots; i++)
		{
			var instance = _dropTarget.Instantiate();
			instance.GetNode<DropTarget>(".").Initialize(i+1);
			_dropTargetsParent.AddChild(instance);
		}
		_httpRequestForGettingExhibitsInDisplay.Request(ApiAddress.MuseumApiPath + "GetAllDisplayArtifacts");
		
		// if ()
		// {
		// 	
		// }
	}
	private void HttpRequestForGettingExhibitsInStoreOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		DeleteChild(_draggablesParent);
		
		string jsonStr = Encoding.UTF8.GetString(body);
		var artifacts = JsonSerializer.Deserialize<List<Artifact>>(jsonStr);
		foreach (var artifact in artifacts)
		{
			var instance = _draggable.Instantiate();
			_draggablesParent.AddChild(instance);
			instance.GetNode<Draggable>(".").Initialize(artifact);
		}

		
	}

	private void DeleteChild(Control node)
	{
		foreach (Node child in node.GetChildren())
		{
			child.QueueFree();
		}
	}

	private void ExitButtonOnPressed()
	{
		Visible = false;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_httpRequestForGettingExhibitsInStore.RequestCompleted -= HttpRequestForGettingExhibitsInStoreOnRequestCompleted;
		_httpRequestForGettingExhibitsInDisplay.RequestCompleted -= HttpRequestForGettingExhibitsInDisplayOnRequestCompleted;
		MuseumActions.ArtifactDroppedOnSlot -= ArtifactDroppedOnSlot;
		MuseumActions.ArtifactRemovedFromSlot -= ArtifactRemovedFromSlot;
		_exitButton.Pressed -= ExitButtonOnPressed;
	}
}
