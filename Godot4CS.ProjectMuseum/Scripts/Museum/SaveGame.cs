using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Service.SaveLoadServices;
using ProjectMuseum.Models;

public partial class SaveGame : Node2D
{
	private HttpRequest _httpRequestForSavingGame;
	private bool _savingGame = false;
	private MuseumRunningDataContainer _museumRunningDataContainer;
	public override void _Ready()
	{
		base._Ready();
		_httpRequestForSavingGame = new HttpRequest();
		AddChild(_httpRequestForSavingGame);
		_httpRequestForSavingGame.RequestCompleted += HttpRequestForSavingGameOnRequestCompleted;
		MuseumActions.OnPlayerSavedGame += SaveGameToJson;
		_museumRunningDataContainer = ServiceRegistry.Resolve<MuseumRunningDataContainer>();

	}

	private void HttpRequestForSavingGameOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		_savingGame = false;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (Input.IsActionJustPressed("save"))
		{
			SaveGameToJson();
		}
	}

	private void SaveGameToJson()
	{
		if (!_savingGame)
		{
			// _httpRequestForSavingGame.Request(ApiAddress.PlayerApiPath + "SaveData");
			var saveData = new SaveData();
			saveData.PlayerInfo = _museumRunningDataContainer.PlayerInfo;
			saveData.MuseumTiles = _museumRunningDataContainer.MuseumTiles;
			var inventoryJson = Godot.FileAccess.Open("res://Game Data/Starting Data/inventory.json", Godot.FileAccess.ModeFlags.Read).GetAsText();
			saveData.Inventory = JsonSerializer.Deserialize<Inventory>(inventoryJson);
			saveData.Exhibits = _museumRunningDataContainer.Exhibits;
			saveData.ArtifactStorage = _museumRunningDataContainer.ArtifactStorage;
			saveData.Time = _museumRunningDataContainer.Time;
			SaveLoadService.Save(saveData);
			GD.Print($"Saved Game at: {DataPath.SaveDataFolderPath}");
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_httpRequestForSavingGame.RequestCompleted -= HttpRequestForSavingGameOnRequestCompleted;
		MuseumActions.OnPlayerSavedGame -= SaveGameToJson;
	}
}
