using Godot;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.ArtifactStorage;

public partial class ArtifactFromInventoryToArtifactStorage : Node2D
{
	private HttpRequest _addArtifactToInventoryHttpRequest;

	public override void _Ready()
	{
		SubscribeToActions();
		CreateHttpRequest();
	}

	private void SubscribeToActions()
	{
		//MineActions.OnArtifactSuccessfullyRetrieved += MoveArtifactFromInventoryToArtifactStorage;
	}

	private void CreateHttpRequest()
	{
		_addArtifactToInventoryHttpRequest = new HttpRequest();
		AddChild(_addArtifactToInventoryHttpRequest);
		_addArtifactToInventoryHttpRequest.RequestCompleted += OnMoveArtifactFromInventoryToArtifactStorageHttpRequestComplete;
	}
	
	private void MoveArtifactFromInventoryToArtifactStorage(Artifact artifact)
	{
		string[] headers = { "Content-Type: application/json"};
		var body = JsonConvert.SerializeObject(artifact);

		_addArtifactToInventoryHttpRequest.Request(ApiAddress.MineApiPath+"AddArtifactToArtifactStorage", headers,
			HttpClient.Method.Post, body);
	}
	
	private void OnMoveArtifactFromInventoryToArtifactStorageHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
	{
		GD.Print($"artifact added to artifact storage");
	}

}