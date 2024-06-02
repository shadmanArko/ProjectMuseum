using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.ArtifactStorage;

public partial class ArtifactFromInventoryToArtifactStorage : Node2D
{
	private HttpRequest _sendAllArtifactsFromInventoryToStorageHttpRequest;

	private InventoryDTO _inventoryDto;

	public override void _Ready()
	{
		SubscribeToActions();
		CreateHttpRequest();
		InitializeDiReference();
	}

	private void InitializeDiReference()
	{
		_inventoryDto = ServiceRegistry.Resolve<InventoryDTO>();
	}

	private void SubscribeToActions()
	{
		MineActions.OnPlayerReachBackToCamp += SendArtifactFromInventoryToStorage;
	}

	private void CreateHttpRequest()
	{
		_sendAllArtifactsFromInventoryToStorageHttpRequest = new HttpRequest();
		AddChild(_sendAllArtifactsFromInventoryToStorageHttpRequest);
		_sendAllArtifactsFromInventoryToStorageHttpRequest.RequestCompleted += OnSendArtifactsFromInventoryToStorageHttpRequestComplete;
	}

	#region From DI Inventory to Artifact Storage (New)

	private void SendArtifactFromInventoryToStorage()
	{
		var itemsToRemove = _inventoryDto.Inventory.InventoryItems.Where(item => item.Type == "Artifact").ToList();
		foreach (var item in itemsToRemove)
			_inventoryDto.Inventory.InventoryItems.Remove(item);
		SendArtifactToStorage(_inventoryDto.Inventory.Artifacts);
	}
	
	private void SendArtifactToStorage(List<Artifact> artifacts)
	{
		string[] headers = { "Content-Type: application/json"};
		var body = JsonConvert.SerializeObject(artifacts);

		_sendAllArtifactsFromInventoryToStorageHttpRequest.Request(ApiAddress.PlayerApiPath+"SendAllArtifactsToArtifactStorage", headers,
			HttpClient.Method.Put, body);
	}
	
	private void OnSendArtifactsFromInventoryToStorageHttpRequestComplete(long result, long responseCode, string[] headers, byte[] body)
	{
		_inventoryDto.Inventory.Artifacts.Clear();
	}

	#endregion

}