using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

public partial class ExhibitEditorUi : Control
{
	[Export] private Button _exitButton;
	[Export] private PackedScene _draggable;
	// [Export] private PackedScene _dropTarget;
	[Export] private PackedScene _dropTargetGrid2X2;
	[Export] private Control _draggablesParent;
	[Export] private Control _dropTargetsParent;
	[Export] private CheckButton _glassCheckButton;
	[Export] private Button _DeleteExhibitButton;
	[Export] private Button _moveExhibitButton;
	private Item _selectedItem;
	private HttpRequest _httpRequestForGettingArtifactsInStore;
	private HttpRequest _httpRequestForGettingArtifactsInDisplay;
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
		_httpRequestForGettingArtifactsInStore = new HttpRequest();
		_httpRequestForGettingArtifactsInDisplay = new HttpRequest();
		_httpRequestForGettingRawArtifactFunctionalData = new HttpRequest();
		_httpRequestForGettingRawArtifactDescriptiveData = new HttpRequest();
		_httpRequestForDeletingExhibit = new HttpRequest();
		AddChild(_httpRequestForGettingArtifactsInStore);
		AddChild(_httpRequestForGettingArtifactsInDisplay);
		AddChild(_httpRequestForGettingRawArtifactFunctionalData);
		AddChild(_httpRequestForGettingRawArtifactDescriptiveData);
		AddChild(_httpRequestForDeletingExhibit);
		_httpRequestForGettingArtifactsInStore.RequestCompleted += HttpRequestForGettingArtifactsInStoreOnRequestCompleted;
		_httpRequestForGettingArtifactsInDisplay.RequestCompleted += HttpRequestForGettingArtifactsInDisplayOnRequestCompleted;
		_httpRequestForGettingRawArtifactFunctionalData.RequestCompleted += HttpRequestForGettingRawArtifactFunctionalDataOnRequestCompleted;
		_httpRequestForGettingRawArtifactDescriptiveData.RequestCompleted += HttpRequestForGettingRawArtifactDescriptiveDataOnRequestCompleted;
		_httpRequestForDeletingExhibit.RequestCompleted += HttpRequestForDeletingExhibitOnRequestCompleted;
		MuseumActions.ArtifactDroppedOnSlot += ArtifactDroppedOnSlot;
		MuseumActions.ArtifactRemovedFromSlot += ArtifactRemovedFromSlot;
		MuseumActions.OnExhibitUpdated += OnExhibitUpdated;
		MuseumActions.PlayStoryScene += PlayStoryScene;
		MuseumActions.DragStarted += DragStarted;
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

	private void OnExhibitUpdated(Exhibit obj)
	{
		if (_selectedExhibit.Id == obj.Id)
		{
			_selectedExhibit = obj;
			GD.Print($"Exhibit after updated {JsonConvert.SerializeObject(_selectedExhibit)}");
		}
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

	private void ArtifactRemovedFromSlot(Artifact artifact, int slotNumber, int gridNumber)
	{
		var artifactSize = GetArtifactSize(artifact.Id);
		// if (slotNumber == 1)
		// {
		// 	_selectedExhibit.ArtifactGridSlots2X2s[gridNumber].Slot0 = "";
		// }else if (slotNumber == 2)
		// {
		// 	_selectedExhibit.ArtifactGridSlots2X2s[gridNumber].Slot1 = "";
		// }else if (slotNumber == 3)
		// {
		// 	_selectedExhibit.ArtifactGridSlots2X2s[gridNumber].Slot2 = "";
		// }else if (slotNumber == 4)
		// {
		// 	_selectedExhibit.ArtifactGridSlots2X2s[gridNumber].Slot3 = "";
		// }
		// GD.Print($"Exhibit after removing artifact {JsonConvert.SerializeObject(_selectedExhibit)}");

		if (_displayArtifacts.Contains(artifact))
		{
			_displayArtifacts.Remove(artifact);
		}
		MuseumActions.ArtifactRemovedFromExhibitSlot?.Invoke(artifact, _selectedItem, slotNumber, gridNumber, artifactSize);
	}
	private void DragStarted(Draggable obj)
	{
		if (obj.IsInGroup("Draggable"))
		{
			GD.Print("Drag Started from editor ui");
			var draggableSize = obj.ArtifactSize;
			if (draggableSize == "Medium")
			{
				HandleMediumSizeArtifactEligibility();
			}
			else if (draggableSize == "Small")
			{
				HandleSmallSizeArtifactEligibility();
			}
			else if (draggableSize == "Tiny")
			{
				HandleTinySizeArtifactEligibility();
			}
		}
		else
		{
			GD.Print("Drag Started but not in draggable group");

		}


	}


	private void HandleTinySizeArtifactEligibility()
	{
		int gridCount = 0;
		foreach (var gridSlots2X2 in _selectedExhibit.ArtifactGridSlots2X2s)
		{

			if (GetArtifactSize(gridSlots2X2.Slot0) == "Medium" || GetArtifactSize(gridSlots2X2.Slot1) == "Medium" ||
				(GetArtifactSize(gridSlots2X2.Slot0) == "Small" && GetArtifactSize(gridSlots2X2.Slot1) == "Small") ||
				(GetArtifactSize(gridSlots2X2.Slot0) == "Tiny" && GetArtifactSize(gridSlots2X2.Slot1) == "Tiny"
				&& GetArtifactSize(gridSlots2X2.Slot2) == "Tiny" && GetArtifactSize(gridSlots2X2.Slot3) == "Tiny")
				)
			{
				//when no slot is empty
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 1);
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 2);
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 3);
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 4);

			}
			else if (gridSlots2X2.Slot0.IsUnassigned() && gridSlots2X2.Slot1.IsUnassigned() &&
					  gridSlots2X2.Slot2.IsUnassigned() && gridSlots2X2.Slot3.IsUnassigned())
			{
				//when all slot is empty
				GD.Print("All slots eligible for tiny object");
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 1);
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 2);
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 3);
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 4);

			}
			else if (GetArtifactSize(gridSlots2X2.Slot0) == "Small")
			{
				//when 1, 3 slot is booked
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 1);
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 3);
				if (GetArtifactSize(gridSlots2X2.Slot1).IsUnassigned())
				{
					MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 2);
				}
				else
				{
					MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 2);
				}

				if (GetArtifactSize(gridSlots2X2.Slot3).IsUnassigned())
				{
					MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 4);
				}
				else
				{
					MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 4);
				}
			}
			else if (GetArtifactSize(gridSlots2X2.Slot1) == "Small")
			{
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 2);
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 4);
				if (GetArtifactSize(gridSlots2X2.Slot0).IsUnassigned())
				{
					MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 1);
				}
				else
				{
					MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 1);
				}

				if (GetArtifactSize(gridSlots2X2.Slot2).IsUnassigned())
				{
					MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 3);
				}
				else
				{
					MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 3);
				}
			}
			else if (GetArtifactSize(gridSlots2X2.Slot0).IsUnassigned())
			{
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 1);
				if (!GetArtifactSize(gridSlots2X2.Slot2).IsUnassigned())
				{
					MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 3);
				}

			}
			else if (GetArtifactSize(gridSlots2X2.Slot1).IsUnassigned())
			{
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 2);
				if (!GetArtifactSize(gridSlots2X2.Slot3).IsUnassigned())
				{
					MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 4);
				}

			}


			gridCount++;
		}
	}

	private void HandleSmallSizeArtifactEligibility()
	{
		int gridCount = 0;
		foreach (var gridSlots2X2 in _selectedExhibit.ArtifactGridSlots2X2s)
		{

			if (GetArtifactSize(gridSlots2X2.Slot0) == "Medium" || GetArtifactSize(gridSlots2X2.Slot1) == "Medium")
			{
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 1);
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 2);
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 3);
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 4);

			}
			else if (gridSlots2X2.Slot0.IsUnassigned() && gridSlots2X2.Slot1.IsUnassigned() &&
				gridSlots2X2.Slot2.IsUnassigned() && gridSlots2X2.Slot3.IsUnassigned())
			{
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 1);
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 2);
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 3);
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 4);

			}
			else if (gridSlots2X2.Slot0.IsUnassigned() && gridSlots2X2.Slot2.IsUnassigned())
			{
				var artifactSize = GetArtifactSize(gridSlots2X2.Slot1);
				if (!artifactSize.IsUnassigned() && artifactSize == "Small")
				{
					MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 1);
					MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 3);

					MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 2);
					MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 4);
				}
			}
			else if (gridSlots2X2.Slot1.IsUnassigned() && gridSlots2X2.Slot3.IsUnassigned())
			{
				var artifactSize = GetArtifactSize(gridSlots2X2.Slot0);
				if (!artifactSize.IsUnassigned() && artifactSize == "Small")
				{
					MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 2);
					MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 4);

					MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 1);
					MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 3);
				}
			}

			gridCount++;
		}
	}

	private string GetArtifactSize(string artifactId)
	{
		foreach (var displayArtifact in _displayArtifacts)
		{
			if (displayArtifact.Id == artifactId)
			{
				var artifactFunctional = _rawArtifactFunctionalDatas.FirstOrDefault(functional => functional.Id == displayArtifact.RawArtifactId);
				if (artifactFunctional != null)
				{
					return artifactFunctional.ObjectSize;
				}
			}
		}
		GD.Print($"Artifact not in display {artifactId}");

		foreach (var artifactInStore in _artifactsInStore)
		{
			if (artifactInStore.Id == artifactId)
			{
				var artifactFunctional = _rawArtifactFunctionalDatas.FirstOrDefault(functional => functional.Id == artifactInStore.RawArtifactId);
				if (artifactFunctional != null)
				{
					return artifactFunctional.ObjectSize;
				}
			}
		}
		GD.Print($"Artifact not in Store {artifactId}");
		return "";
	}

	private void HandleMediumSizeArtifactEligibility()
	{
		int gridCount = 0;
		foreach (var gridSlots2X2 in _selectedExhibit.ArtifactGridSlots2X2s)
		{
			if (gridSlots2X2.Slot0.IsUnassigned() && gridSlots2X2.Slot1.IsUnassigned() &&
				gridSlots2X2.Slot2.IsUnassigned() && gridSlots2X2.Slot3.IsUnassigned())
			{
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 1);
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 2);
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 3);
				MuseumActions.OnMakeGridSlotEligible?.Invoke(gridCount, 4);

			}
			else
			{
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 1);
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 2);
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 3);
				MuseumActions.OnMakeGridSlotDisable?.Invoke(gridCount, 4);
			}

			gridCount++;
		}
	}

	private List<Artifact> _displayArtifacts;
	private List<Artifact> _artifactsInStore;
	private void HttpRequestForGettingArtifactsInDisplayOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var artifacts = JsonSerializer.Deserialize<List<Artifact>>(jsonStr);
		_displayArtifacts = artifacts;
		foreach (var artifact in artifacts)
		{
			if (artifact == null || _selectedExhibit == null) continue;
			int gridCount = 0;
			foreach (var gridSlots2X2 in _selectedExhibit.ArtifactGridSlots2X2s)
			{
				if (artifact.Id == gridSlots2X2.Slot0)
				{
					var instance = _draggable.Instantiate();
					_dropTargetsParent.GetChildren()[gridCount].GetChildren()[0].AddChild(instance);
					_dropTargetsParent.GetChildren()[gridCount].GetChildren()[0].GetNode<DropTarget>(".").hasEmptySlot = false;
					instance.GetNode<Draggable>(".").Initialize(artifact, _rawArtifactDescriptiveDatas, _rawArtifactFunctionalDatas);
				}
				else if (artifact.Id == gridSlots2X2.Slot1)
				{
					var instance = _draggable.Instantiate();
					_dropTargetsParent.GetChildren()[gridCount].GetChildren()[1].AddChild(instance);
					_dropTargetsParent.GetChildren()[gridCount].GetChildren()[1].GetNode<DropTarget>(".").hasEmptySlot = false;
					instance.GetNode<Draggable>(".").Initialize(artifact, _rawArtifactDescriptiveDatas, _rawArtifactFunctionalDatas);
				}
				else if (artifact.Id == gridSlots2X2.Slot2)
				{
					var instance = _draggable.Instantiate();
					_dropTargetsParent.GetChildren()[gridCount].GetChildren()[2].AddChild(instance);
					_dropTargetsParent.GetChildren()[gridCount].GetChildren()[2].GetNode<DropTarget>(".").hasEmptySlot = false;
					instance.GetNode<Draggable>(".").Initialize(artifact, _rawArtifactDescriptiveDatas, _rawArtifactFunctionalDatas);
				}
				else if (artifact.Id == gridSlots2X2.Slot3)
				{
					var instance = _draggable.Instantiate();
					_dropTargetsParent.GetChildren()[gridCount].GetChildren()[3].AddChild(instance);
					_dropTargetsParent.GetChildren()[gridCount].GetChildren()[3].GetNode<DropTarget>(".").hasEmptySlot = false;
					instance.GetNode<Draggable>(".").Initialize(artifact, _rawArtifactDescriptiveDatas, _rawArtifactFunctionalDatas);
				}

				gridCount++;
			}
		}
	}

	private void ArtifactDroppedOnSlot(Artifact artifact, string size, int slotNumber, int gridNumber)
	{
		var artifactSize = GetArtifactSize(artifact.Id);
		// if (artifactSize == "Small")
		// {
		// 	if (slotNumber == 3 || slotNumber == 1)
		// 	{
		// 		slotNumber = 1;
		// 		_selectedExhibit.ArtifactGridSlots2X2s[gridNumber].Slot0 = artifact.Id;
		// 	}else if (slotNumber == 2 || slotNumber == 4)
		// 	{
		// 		slotNumber = 2;
		// 		_selectedExhibit.ArtifactGridSlots2X2s[gridNumber].Slot1 = artifact.Id;
		// 	}
		// }
		// else if (artifactSize == "Medium")
		// {
		// 	if (slotNumber == 1 || slotNumber == 2 || slotNumber == 3 || slotNumber == 4)
		// 	{
		// 		slotNumber = 1;
		// 		_selectedExhibit.ArtifactGridSlots2X2s[gridNumber].Slot0 = artifact.Id;
		// 	}
		// }
		// else if (artifactSize == "Tiny")
		// {
		// 	if (slotNumber == 1)
		// 	{
		// 		_selectedExhibit.ArtifactGridSlots2X2s[gridNumber].Slot0 = artifact.Id;
		// 	}else if (slotNumber == 2)
		// 	{
		// 		_selectedExhibit.ArtifactGridSlots2X2s[gridNumber].Slot1 = artifact.Id;
		// 	}else if (slotNumber == 3)
		// 	{
		// 		_selectedExhibit.ArtifactGridSlots2X2s[gridNumber].Slot2 = artifact.Id;
		// 	}else if (slotNumber == 4)
		// 	{
		// 		_selectedExhibit.ArtifactGridSlots2X2s[gridNumber].Slot3 = artifact.Id;
		// 	}
		// }


		if (!_displayArtifacts.Contains(artifact))
		{
			_displayArtifacts.Add(artifact);
		}
		MuseumActions.ArtifactDroppedOnExhibitSlot?.Invoke(artifact, _selectedItem, slotNumber, gridNumber, artifactSize);
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
		var numberOfGrids = item.numberOfTilesItTakes < 4 ? 1 : 4;
		for (int gridNumber = 0; gridNumber < numberOfGrids; gridNumber++)
		{
			if (_dropTargetGrid2X2 == null)
			{
				GD.Print("Drop target grid is null.");
			}

			var instance = _dropTargetGrid2X2.Instantiate();
			int childCount = 0;
			foreach (var child in instance.GetChildren())
			{
				childCount++;
				child.GetNode<DropTarget>(".").Initialize(childCount, gridNumber, _rawArtifactDescriptiveDatas, _rawArtifactFunctionalDatas);
			}

			_dropTargetsParent.AddChild(instance);
		}


		// if ()
		// {
		// 	
		// }
	}
	private void HttpRequestForGettingArtifactsInStoreOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
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
		_httpRequestForGettingArtifactsInStore.RequestCompleted -= HttpRequestForGettingArtifactsInStoreOnRequestCompleted;
		_httpRequestForGettingArtifactsInDisplay.RequestCompleted -= HttpRequestForGettingArtifactsInDisplayOnRequestCompleted;
		_httpRequestForGettingRawArtifactFunctionalData.RequestCompleted -= HttpRequestForGettingRawArtifactFunctionalDataOnRequestCompleted;
		_httpRequestForGettingRawArtifactDescriptiveData.RequestCompleted -= HttpRequestForGettingRawArtifactDescriptiveDataOnRequestCompleted;
		MuseumActions.ArtifactDroppedOnSlot -= ArtifactDroppedOnSlot;
		MuseumActions.ArtifactRemovedFromSlot -= ArtifactRemovedFromSlot;
		_DeleteExhibitButton.Pressed -= DeleteExhibitButtonOnPressed;
		MuseumActions.PlayStoryScene -= PlayStoryScene;
		MuseumActions.DragStarted -= DragStarted;
		_exitButton.Pressed -= ExitButtonOnPressed;
		_glassCheckButton.Pressed -= GlassCheckButtonOnPressed;

	}

	private void PlayStoryScene(int obj)
	{
		ExitButtonOnPressed();
	}
}
