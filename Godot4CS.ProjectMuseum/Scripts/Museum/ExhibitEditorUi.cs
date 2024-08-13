using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Managers;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

public partial class ExhibitEditorUi : Control
{
	[Export] private Button _exitButton;
	[Export] private PackedScene _draggable;
	[Export] private PackedScene _dropTarget;
	[Export] private Control _draggablesParent;
	[Export] private Control _dropTargetsParent;
	[Export] private CheckButton _glassCheckButton;
	[Export] private Button _DeleteExhibitButton;
	[Export] private Button _moveExhibitButton;
	private Item _selectedItem;
	private HttpRequest _httpRequestForGettingExhibitsInStore;
	private HttpRequest _httpRequestForGettingExhibitsInDisplay;
	private HttpRequest _httpRequestForGettingRawArtifactFunctionalData;
	private HttpRequest _httpRequestForGettingRawArtifactDescriptiveData;
	private HttpRequest _httpRequestForDeletingExhibit;
	private static List<RawArtifactDescriptive> _rawArtifactDescriptiveDatas;
	private static List<RawArtifactFunctional> _rawArtifactFunctionalDatas;
	private Exhibit _selectedExhibit;
	private MuseumRunningDataContainer _museumRunningDataContainer;
	
    // Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		_httpRequestForGettingExhibitsInStore = new HttpRequest();
		_httpRequestForGettingExhibitsInDisplay = new HttpRequest();
		_httpRequestForGettingRawArtifactFunctionalData = new HttpRequest();
		_httpRequestForGettingRawArtifactDescriptiveData = new HttpRequest();
		_httpRequestForDeletingExhibit = new HttpRequest();
		AddChild(_httpRequestForGettingExhibitsInStore);
		AddChild(_httpRequestForGettingExhibitsInDisplay);
		AddChild(_httpRequestForGettingRawArtifactFunctionalData);
		AddChild(_httpRequestForGettingRawArtifactDescriptiveData);
		AddChild(_httpRequestForDeletingExhibit);
		_httpRequestForGettingExhibitsInStore.RequestCompleted += HttpRequestForGettingExhibitsInStoreOnRequestCompleted;
		_httpRequestForGettingExhibitsInDisplay.RequestCompleted += HttpRequestForGettingExhibitsInDisplayOnRequestCompleted;
		_httpRequestForGettingRawArtifactFunctionalData.RequestCompleted += HttpRequestForGettingRawArtifactFunctionalDataOnRequestCompleted;
		_httpRequestForGettingRawArtifactDescriptiveData.RequestCompleted += HttpRequestForGettingRawArtifactDescriptiveDataOnRequestCompleted;
		_httpRequestForDeletingExhibit.RequestCompleted += HttpRequestForDeletingExhibitOnRequestCompleted;
		MuseumActions.ArtifactDroppedOnSlot += ArtifactDroppedOnSlot;
		MuseumActions.ArtifactRemovedFromSlot += ArtifactRemovedFromSlot;
		MuseumActions.PlayStoryScene += PlayStoryScene;
		_exitButton.Pressed += ExitButtonOnPressed;
		_glassCheckButton.Pressed += GlassCheckButtonOnPressed;
		_DeleteExhibitButton.Pressed += DeleteExhibitButtonOnPressed;
		_moveExhibitButton.Pressed += MoveExhibitButtonOnPressed;
		GetRawDatas();
		await Task.Delay(1000);
		_museumRunningDataContainer = ServiceRegistry.Resolve<MuseumRunningDataContainer>();
	}

	private void GetRawDatas()
	{
		var rawArtifactDescriptiveJson = Godot.FileAccess.Open("res://Game Data/RawArtifactData/RawArtifactDescriptiveData/RawArtifactDescriptiveDataEnglish.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
		_rawArtifactDescriptiveDatas = JsonSerializer.Deserialize<List<RawArtifactDescriptive>>(rawArtifactDescriptiveJson);
		MuseumActions.OnRawArtifactDescriptiveDataLoaded?.Invoke(_rawArtifactDescriptiveDatas);
		
		var rawArtifactFunctionalJson = Godot.FileAccess.Open("res://Game Data/RawArtifactData/RawArtifactFunctionalData/RawArtifactFunctionalData.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
		_rawArtifactFunctionalDatas = JsonSerializer.Deserialize<List<RawArtifactFunctional>>(rawArtifactFunctionalJson);
		MuseumActions.OnRawArtifactFunctionalDataLoaded?.Invoke(_rawArtifactFunctionalDatas);
		
		// _httpRequestForGettingRawArtifactFunctionalData.Request(ApiAddress.MineApiPath + "GetAllRawArtifactFunctional");
		// _httpRequestForGettingRawArtifactDescriptiveData.Request(ApiAddress.MineApiPath + "GetAllRawArtifactDescriptive");
	}

	private void MoveExhibitButtonOnPressed()
	{
		Visible = false;
		MuseumActions.OnMakeExhibitFloatForMoving?.Invoke(_selectedExhibit.Id);
	}

	private void DeleteExhibitButtonOnPressed()
	{
		Visible = false;
		_httpRequestForDeletingExhibit.Request(ApiAddress.MuseumApiPath + $"DeleteExhibit?exhibitId={_selectedExhibit.Id}");
	}

	private void HttpRequestForDeletingExhibitOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var tilesWithExhibitDto = JsonSerializer.Deserialize<TilesWithExhibitDto>(jsonStr);
		_museumRunningDataContainer.MuseumTiles = tilesWithExhibitDto.MuseumTiles;
		_museumRunningDataContainer.Exhibits = tilesWithExhibitDto.Exhibits;
		MuseumActions.OnExhibitDeleted?.Invoke(_selectedExhibit.Id);
	}

	private void HttpRequestForGettingRawArtifactDescriptiveDataOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_rawArtifactDescriptiveDatas = JsonSerializer.Deserialize<List<RawArtifactDescriptive>>(jsonStr);
		MuseumActions.OnRawArtifactDescriptiveDataLoaded?.Invoke(_rawArtifactDescriptiveDatas);
	}

	private void HttpRequestForGettingRawArtifactFunctionalDataOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		_rawArtifactFunctionalDatas = JsonSerializer.Deserialize<List<RawArtifactFunctional>>(jsonStr);
		MuseumActions.OnRawArtifactFunctionalDataLoaded?.Invoke(_rawArtifactFunctionalDatas);
	}

	private void GlassCheckButtonOnPressed()
	{
		_selectedItem.EnableGlass(_glassCheckButton.ButtonPressed);
	}

	private void ArtifactRemovedFromSlot(Artifact artifact, int slotNumber)
	{
		MuseumActions.ArtifactRemovedFromExhibitSlot?.Invoke(artifact, _selectedItem, slotNumber);
	}

	private void HttpRequestForGettingExhibitsInDisplayOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var artifacts = JsonSerializer.Deserialize<List<Artifact>>(jsonStr);
		AfterGettingDisplayArtifacts(artifacts);
	}

	private void AfterGettingDisplayArtifacts(List<Artifact> artifacts)
	{
		foreach (var artifact in artifacts)
		{
			if (artifact == null || _selectedExhibit == null) continue;

			if (artifact.Id == _selectedExhibit.ExhibitArtifactSlot1)
			{
				var instance = _draggable.Instantiate();
				_dropTargetsParent.GetChildren()[0].AddChild(instance);
				instance.GetNode<Draggable>(".")
					.Initialize(artifact, _rawArtifactDescriptiveDatas, _rawArtifactFunctionalDatas);
			}
			else if (artifact.Id == _selectedExhibit.ExhibitArtifactSlot2)
			{
				var instance = _draggable.Instantiate();
				_dropTargetsParent.GetChildren()[1].AddChild(instance);
				instance.GetNode<Draggable>(".")
					.Initialize(artifact, _rawArtifactDescriptiveDatas, _rawArtifactFunctionalDatas);
			}
			else if (artifact.Id == _selectedExhibit.ExhibitArtifactSlot3)
			{
				var instance = _draggable.Instantiate();
				_dropTargetsParent.GetChildren()[2].AddChild(instance);
				instance.GetNode<Draggable>(".")
					.Initialize(artifact, _rawArtifactDescriptiveDatas, _rawArtifactFunctionalDatas);
			}
			else if (artifact.Id == _selectedExhibit.ExhibitArtifactSlot4)
			{
				var instance = _draggable.Instantiate();
				_dropTargetsParent.GetChildren()[3].AddChild(instance);
				instance.GetNode<Draggable>(".")
					.Initialize(artifact, _rawArtifactDescriptiveDatas, _rawArtifactFunctionalDatas);
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
		_httpRequestForGettingExhibitsInStore.CancelRequest();
		_httpRequestForGettingExhibitsInDisplay.CancelRequest();
		// _httpRequestForGettingExhibitsInStore.Request(ApiAddress.MuseumApiPath + "GetAllArtifactsInStorage");
		var artifactsInStore = MuseumReferenceManager.Instance.ArtifactStoreServices.GetAllArtifacts();
		AfterGettingAftifactsInStore(artifactsInStore);
		DeleteChild(_dropTargetsParent);
		_selectedItem = item;
		_glassCheckButton.ButtonPressed = _selectedItem.IsGlassEnabled();
		var numberOfSlots = item.numberOfTilesItTakes < 4 ? 1 : 4;
		for (int i = 0; i < numberOfSlots; i++)
		{
			var instance = _dropTarget.Instantiate();
			// foreach (var child in instance.GetChildren())
			// {
			// 	
			// 	
			// }
			instance.GetNode<DropTarget>(".").Initialize(i+1, _rawArtifactDescriptiveDatas, _rawArtifactFunctionalDatas);
			_dropTargetsParent.AddChild(instance);
		}
		
		
		// if ()
		// {
		// 	
		// }
	}
	private void HttpRequestForGettingExhibitsInStoreOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		
		
		string jsonStr = Encoding.UTF8.GetString(body);
		var artifacts = JsonSerializer.Deserialize<List<Artifact>>(jsonStr);
		AfterGettingAftifactsInStore(artifacts);
	}

	private void AfterGettingAftifactsInStore(List<Artifact> artifacts)
	{
		DeleteChild(_draggablesParent);
		foreach (var artifact in artifacts)
		{
			var instance = _draggable.Instantiate();
			_draggablesParent.AddChild(instance);
			instance.GetNode<Draggable>(".")
				.Initialize(artifact, _rawArtifactDescriptiveDatas, _rawArtifactFunctionalDatas);
		}

		_httpRequestForGettingExhibitsInDisplay.Request(ApiAddress.MuseumApiPath + "GetAllDisplayArtifacts");
		var displayArtifacts = MuseumReferenceManager.Instance.DisplayArtifactServices.GetAllArtifacts();
		AfterGettingDisplayArtifacts(displayArtifacts);
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
		_httpRequestForGettingRawArtifactFunctionalData.RequestCompleted -= HttpRequestForGettingRawArtifactFunctionalDataOnRequestCompleted;
		_httpRequestForGettingRawArtifactDescriptiveData.RequestCompleted -= HttpRequestForGettingRawArtifactDescriptiveDataOnRequestCompleted;
		MuseumActions.ArtifactDroppedOnSlot -= ArtifactDroppedOnSlot;
		MuseumActions.ArtifactRemovedFromSlot -= ArtifactRemovedFromSlot;
		_DeleteExhibitButton.Pressed -= DeleteExhibitButtonOnPressed;
		MuseumActions.PlayStoryScene -= PlayStoryScene;
		_exitButton.Pressed -= ExitButtonOnPressed;
		_glassCheckButton.Pressed -= GlassCheckButtonOnPressed;

	}

	private void PlayStoryScene(int obj)
	{
		ExitButtonOnPressed();
	}
}
