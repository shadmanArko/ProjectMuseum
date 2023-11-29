using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

public partial class ExhibitEditorUi : Control
{
	[Export] private Button _exitButton;
	[Export] private PackedScene _draggable;
	[Export] private Control _draggablesParent;

	private HttpRequest _httpRequestForGettingExhibitsInStore;
    // Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_httpRequestForGettingExhibitsInStore = new HttpRequest();
		AddChild(_httpRequestForGettingExhibitsInStore);
		_httpRequestForGettingExhibitsInStore.RequestCompleted += HttpRequestForGettingExhibitsInStoreOnRequestCompleted;
		_httpRequestForGettingExhibitsInStore.Request(ApiAddress.MuseumApiPath + "GetAllArtifactsInStorage");
		_exitButton.Pressed += ExitButtonOnPressed;
	}

	private void HttpRequestForGettingExhibitsInStoreOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
	{
		string jsonStr = Encoding.UTF8.GetString(body);
		var artifacts = JsonSerializer.Deserialize<List<Artifact>>(jsonStr);
		foreach (var artifact in artifacts)
		{
			var instance = _draggable.Instantiate();
			_draggablesParent.AddChild(instance);
			instance.GetNode<Draggable>(".").Initialize(artifact);
		}
	}

	private void ExitButtonOnPressed()
	{
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
