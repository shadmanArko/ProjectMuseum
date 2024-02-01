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
	[Export] private Button _startNewGameButton;
	[Export] private Button _continueButton;
	[Export] private Button _optionsButton;
	[Export] private Button _exitButtonButton;
	private HttpRequest _httpRequestForGettingMainMenuMiscellaneousData;
	
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
		MainMenuMiscellaneousData mainMenuMiscellaneousData = JsonSerializer.Deserialize<MainMenuMiscellaneousData>(jsonStr);
		// SetUpMainMenuMiscellaneousData(mainMenuMiscellaneousData);
	}

	private void SetUpMainMenuMiscellaneousData(MainMenuMiscellaneousData mainMenuMiscellaneousData)
	{
		_startNewGameButton.Text = mainMenuMiscellaneousData.StartNewGameText;
		_optionsButton.Text = mainMenuMiscellaneousData.OptionsText;
		_continueButton.Text = mainMenuMiscellaneousData.ContinueText;
		_exitButtonButton.Text = mainMenuMiscellaneousData.ExitText;
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