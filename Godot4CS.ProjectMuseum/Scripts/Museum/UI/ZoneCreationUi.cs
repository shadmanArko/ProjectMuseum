using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using Newtonsoft.Json;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

public partial class ZoneCreationUi : Control
{
	[Export] private LineEdit _zoneName;
	[Export] private ColorPickerButton _colorPickerButton;
	[Export] private Button _createZoneButton;
	[Export] private Button _cancelZoneButton;
	private MuseumTileContainer _museumTileContainer;
	private List<string> _selectedTileIds = new List<string>();

	private HttpRequest _httpRequestForCreatingZone;
	private HttpRequest _httpRequestForGettingAllZones;
	// Called when the node enters the scene tree for the first time.
	public override async void _Ready()
	{
		_createZoneButton.Pressed += DisableZoneCreationUi;
		_createZoneButton.Pressed += CreateZoneButtonOnPressed;
		_cancelZoneButton.Pressed += DisableZoneCreationUi;
		_colorPickerButton.ColorChanged += ColorPickerButtonOnColorChanged;
		_httpRequestForCreatingZone = new HttpRequest();
		_httpRequestForGettingAllZones = new HttpRequest();
		AddChild(_httpRequestForCreatingZone);
		AddChild(_httpRequestForGettingAllZones);
		_httpRequestForCreatingZone.RequestCompleted += HttpRequestForCreatingZoneOnRequestCompleted;
		_httpRequestForGettingAllZones.RequestCompleted += HttpRequestForGettingAllZonesOnRequestCompleted;

		await Task.Delay(1000);
		_museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();

	}

	private void HttpRequestForGettingAllZonesOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		// GD.Print($"Updated Museum Zone{jsonStr}");
		var museumZones = JsonSerializer.Deserialize<List<MuseumZone>>(jsonStr);
		var tileMap = GameManager.TileMap;
		int layerCount = 2;
		foreach (var museumZone in museumZones)
		{
			if (tileMap.GetLayersCount() <= layerCount)
			{
				tileMap.AddLayer(-1);
			}
			tileMap.ClearLayer(layerCount);
			foreach (var tile in _museumTileContainer.MuseumTiles)
			{
				if (museumZone.OccupiedMuseumTileIds.Contains(tile.Id))
				{
					tileMap.SetCell(layerCount, new Vector2I(tile.XPosition, tile.YPosition), 14, new Vector2I(8, 0));
				}
			}

			tileMap.SetLayerModulate(layerCount, ParseColorFromString(museumZone.Color));
			layerCount++;
		}
	}

	private void HttpRequestForCreatingZoneOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		GD.Print($"Updated Museum Zone{jsonStr}");
		var museumZone = JsonSerializer.Deserialize<MuseumZone>(jsonStr);
		_httpRequestForGettingAllZones.Request(ApiAddress.MuseumApiPath + "GetAllZones");
	}
	
	private void CreateZoneButtonOnPressed()
	{
		MuseumZone museumZone = new MuseumZone();
		museumZone.Id = "string";
		museumZone.Type = "string";
		museumZone.Name = _zoneName.Text;
		museumZone.Color = _colorPickerButton.Color.ToString();
		museumZone.OccupiedMuseumTileIds = _selectedTileIds;
		string[] headers = { "Content-Type: application/json"};
		var body = JsonConvert.SerializeObject(museumZone);
		string url = ApiAddress.MuseumApiPath + "CreateNewZone";
		_httpRequestForCreatingZone.Request(url, headers, HttpClient.Method.Put, body);
	}
	private Color ParseColorFromString(string colorString)
	{
		// Remove parentheses and split the string by commas
		string[] components = colorString.Trim('(', ')').Split(',');

		// Parse each component and create a Color object
		float r = float.Parse(components[0]);
		float g = float.Parse(components[1]);
		float b = float.Parse(components[2]);
		float a = float.Parse(components[3]);

		return new Color(r, g, b, a);
	}
	private void ColorPickerButtonOnColorChanged(Color color)
	{
		MuseumActions.OnZoneColorChanged?.Invoke(color);
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void EnableZoneCreationUi()
	{
		Visible = true;
		_colorPickerButton.Color = Colors.White;
	}

	private void DisableZoneCreationUi()
	{
		Visible = false;
		MuseumActions.OnZoneCreationUiClosed?.Invoke();
	}
	private void OnSelectTilesForZone(List<string> obj)
	{
		EnableZoneCreationUi();
		_selectedTileIds = obj;
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		MuseumActions.OnSelectTilesForZone += OnSelectTilesForZone;
		MuseumActions.OnNotSelectingEnoughTiles += DisableZoneCreationUi;
	}
	
	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnSelectTilesForZone -= OnSelectTilesForZone;
		MuseumActions.OnNotSelectingEnoughTiles -= DisableZoneCreationUi;
	}
}
