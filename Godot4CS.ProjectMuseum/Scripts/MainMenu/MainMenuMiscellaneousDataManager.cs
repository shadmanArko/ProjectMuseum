using System.Text;
using Godot;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Time = ProjectMuseum.Models.Time;

namespace Godot4CS.ProjectMuseum.Scripts.MainMenu;

public partial class MainMenuMiscellaneousDataManager : Node2D
{
	private HttpRequest _httpRequestForGettingMainMenuMiscellaneousData;

	[Export] private string _languageKey = "en";

	private MainMenuMiscellaneousData _mainMenuMiscellaneousData;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_httpRequestForGettingMainMenuMiscellaneousData = new HttpRequest();
		AddChild(_httpRequestForGettingMainMenuMiscellaneousData);
		_httpRequestForGettingMainMenuMiscellaneousData.RequestCompleted += HttpRequestForGettingMainMenuMiscellaneousDataOnRequestCompleted;
		_httpRequestForGettingMainMenuMiscellaneousData.Request(ApiAddress.MiscellaneousDataApiPath +
		                                                        "GetMainMenuMiscellaneousData");
	}

	private void HttpRequestForGettingMainMenuMiscellaneousDataOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		GD.Print(jsonStr);
		_mainMenuMiscellaneousData = JsonSerializer.Deserialize<MainMenuMiscellaneousData>(jsonStr);
		SetUpMainMenuMiscellaneousData(_mainMenuMiscellaneousData);
	}

	private void SetUpMainMenuMiscellaneousData(MainMenuMiscellaneousData mainMenuMiscellaneousData)
	{
		

		TraverseSceneTree(GetTree().Root);
	}
	private void TraverseSceneTree(Node node)
	{
		foreach (Node child in node.GetChildren())
		{
			// Check if the node is a TextField
			if (child is Button lineEdit)
			{
				lineEdit.Text = GetLocalizedText(lineEdit.Text);
				string text = lineEdit.Text;
				GD.Print($"Found Text {text}");
			}

			// Recursive call to traverse children
			TraverseSceneTree(child);
		}
	}

	private string GetLocalizedText(string lineEditText)
	{
		var translations = _mainMenuMiscellaneousData.Translations;
		return translations[lineEditText][_languageKey];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_httpRequestForGettingMainMenuMiscellaneousData.RequestCompleted -= HttpRequestForGettingMainMenuMiscellaneousDataOnRequestCompleted;

	}
}