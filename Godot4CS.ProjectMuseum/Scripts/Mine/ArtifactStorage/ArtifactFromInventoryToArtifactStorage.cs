using Godot;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.ArtifactStorage;

public partial class ArtifactFromInventoryToArtifactStorage : Node2D
{
	private HttpRequest _sendAllArtifactFromInventoryToArtifactStorageHttpRequest;

	public override void _Ready()
	{
		SubscribeToActions();
		CreateHttpRequest();
	}

	private void SubscribeToActions()
	{
		MineActions.OnPlayerReachBackToCamp += SendAllArtifactFromInventoryToArtifactStorage;
	}

	private void CreateHttpRequest()
	{
		_sendAllArtifactFromInventoryToArtifactStorageHttpRequest = new HttpRequest();
		AddChild(_sendAllArtifactFromInventoryToArtifactStorageHttpRequest);
		_sendAllArtifactFromInventoryToArtifactStorageHttpRequest.RequestCompleted += OnSendAllArtifactFromInventoryToArtifactStorageHttpRequestComplete;
	}
	
	private void SendAllArtifactFromInventoryToArtifactStorage()
	{
		_sendAllArtifactFromInventoryToArtifactStorageHttpRequest.Request(ApiAddress.PlayerApiPath+"SendAllArtifactsFromInventoryToArtifactStorage");
	}
	
	private void OnSendAllArtifactFromInventoryToArtifactStorageHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print($"artifact added to artifact storage");
	}

}