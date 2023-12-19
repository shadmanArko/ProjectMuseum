using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;
using Equipables = Godot4CS.ProjectMuseum.Scripts.Mine.Enum.Equipables;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class ToolbarSelector : Node
{
	private RawArtifactDTO _rawArtifactDto;
	
	
	private Inventory _inventory;
	private HttpRequest _getPlayerInventoryHttpRequest;

	[Export] private string _toolbarSlotScenePath;
	
	private List<ToolbarSlot> _toolbarSlots;

	public override void _EnterTree()
	{
		CreateHttpRequest();
		InitializeDiReferences();
	}

	public override void _Ready()
	{
		SubscribeToActions();

		_toolbarSlots = new List<ToolbarSlot>();
		GetPlayerInventory();
	}
	
	private void InitializeDiReferences()
	{
		_rawArtifactDto = ServiceRegistry.Resolve<RawArtifactDTO>();
		GD.Print("Raw artifact functional resolved from Toolbar Selector Script");
	}

	private void CreateHttpRequest()
	{
		_getPlayerInventoryHttpRequest = new HttpRequest();
		AddChild(_getPlayerInventoryHttpRequest);
		_getPlayerInventoryHttpRequest.RequestCompleted += OnGetPlayerInventoryHttpRequestComplete;
	}
	
	private void SubscribeToActions()
	{
		MineActions.OnToolbarSlotChanged += SelectItem;
	}

	#region Player Inventory

	private void GetPlayerInventory()
	{
		var url = ApiAddress.PlayerApiPath+"GetInventory";
		_getPlayerInventoryHttpRequest.Request(url);
	}
	
	private void OnGetPlayerInventoryHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
	{
		var jsonStr = Encoding.UTF8.GetString(body);
		_inventory = JsonSerializer.Deserialize<Inventory>(jsonStr);

		for (var i = 0; i < _inventory.SlotsUnlocked; i++)
		{
			var toolbarSlot = ResourceLoader.Load<PackedScene>(_toolbarSlotScenePath).Instantiate() as ToolbarSlot;
			if (toolbarSlot == null)
			{
				GD.PrintErr($"error instantiating slot");
				continue;
			}
			
			GD.Print("Instantiating toolbar slots "+i);
            
			AddChild(toolbarSlot);
			_toolbarSlots.Add(toolbarSlot);
		}

		foreach (var equipable in _inventory.Equipables)
		{
			_toolbarSlots[equipable.Slot].SetItemTexture(equipable.PngPath);
		}
		
		GD.Print(_rawArtifactDto.RawArtifactFunctionals.Count);
		foreach (var artifact in _inventory.Artifacts)
		{
			var rawArtifactFunctional =
				_rawArtifactDto.RawArtifactFunctionals.FirstOrDefault(rawArtifactFunctional => rawArtifactFunctional.Id == artifact.RawArtifactId);
			if (rawArtifactFunctional == null)
			{
				GD.Print("Artifact Not Found");
				continue;
			}
			_toolbarSlots[artifact.Slot].SetItemTexture(rawArtifactFunctional.SmallImageLocation);
		}
	}

	#endregion
    
	private void SelectItem(Equipables equipable)
	{
		GD.Print($"eqipable: {(int) equipable}");
		DeselectAllItems();
		_toolbarSlots[(int) equipable].SetItemAsSelected();
	}


	private void DeselectAllItems()
	{
		foreach (var slot in _toolbarSlots)
			slot.SetItemAsDeselected();
	}
}