using Godot;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Newtonsoft.Json;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.ArtifactStorage;

public partial class ArtifactFromMineToInventory : Node2D
{
    private HttpRequest _addArtifactToInventoryHttpRequest;
    
    public override void _Ready()
    {
        SubscribeToActions();
        CreateHttpRequest();
    }
    
    private void CreateHttpRequest()
    {
        _addArtifactToInventoryHttpRequest = new HttpRequest();
        AddChild(_addArtifactToInventoryHttpRequest);
        _addArtifactToInventoryHttpRequest.RequestCompleted += OnAddArtifactFromMineToInventory;
    }

    private void SubscribeToActions()
    {
        MineActions.OnArtifactSuccessfullyRetrieved += AddArtifactFromMineToInventory;
    }
    
    private void AddArtifactFromMineToInventory(Artifact artifact)
    {
        string[] headers = { "Content-Type: application/json"};
        var body = JsonConvert.SerializeObject(artifact);

        _addArtifactToInventoryHttpRequest.Request(ApiAddress.MineApiPath+"SendArtifactToInventory", headers,
            HttpClient.Method.Post, body);
        
        GD.Print($"HTTP REQUEST FOR ADDING ARTIFACT TO INVENTORY (3)");
    }
    
    private void OnAddArtifactFromMineToInventory(long result, long responseCode, string[] headers, byte[] body)
    {
        GD.Print($"ARTIFACT SUCCESSFULLY ADDED TO INVENTORY!!! (4)");
    }
}