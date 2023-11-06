using Godot;

public partial class NewGameSetupUi : Control
{
	[Export] public Button StartButton;

	[Export] public LineEdit LineEdit;
	[Export] public OptionButton OptionButton;

	[Export] public CheckButton CheckButton;

	private HttpRequest _httpRequest;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		StartButton.Pressed += StartButtonOnPressed;
		
		
		 _httpRequest = new HttpRequest();
		AddChild(_httpRequest);
		_httpRequest.RequestCompleted += OnNewGameSetupRequestComplete;
	}

	private void StartButtonOnPressed()
	{
		
		GD.Print($"Name: {LineEdit.Text}, Gender: {OptionButton.Text}, Tutorial: { CheckButton.ButtonPressed}" );
		string body = Json.Stringify(new Godot.Collections.Dictionary
		{
			{ "Id", "string" },
			{ "Name", LineEdit.Text },
			{ "Gender", OptionButton.Text },
			{ "Tutorial", CheckButton.ButtonPressed }
			

		});
		GD.Print(body);
		string[] headers = { "Content-Type: application/json"};
		Error error = _httpRequest.Request("http://localhost:5178/api/MuseumTile/PostPlayerInfo", headers,
			HttpClient.Method.Post, body);
		if (error != Error.Ok)
		{
			GD.Print("Error not ok");
		}
		else
		{
			GD.Print("Error ok");
		}

		
	}

	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void OnNewGameSetupRequestComplete(long result, long responsecode, string[] headers, byte[] body)
	{
		GetTree().ChangeSceneToFile("res://Tests/Scenes/museum_drag_and_drop.tscn");
	}
}
