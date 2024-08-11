using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Loading_Bar;
using Godot4CS.ProjectMuseum.Scripts.Museum.DragAndDrop;
using Godot4CS.ProjectMuseum.Scripts.Museum.Managers;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum;

public partial class ExhibitController : Node2D
{
    [Export] private ExhibitEditorUi _exhibitEditorUi;
    [Export] private Node2D _itemsParent;
    private HttpRequest _httpRequestForGettingAllExhibits;
    private HttpRequest _httpRequestForGettingAllDisplayArtifacts;
    private PackedScene item1;
    private PackedScene item2;
    private PackedScene item3;
    private PackedScene item4;
    private List<Artifact> _displayArtifacts;
    private MuseumRunningDataContainer _museumRunningDataContainer;

    [Export] private LoadingBarManager _loadingBarManager;
    public override async void _Ready()
    {
        _loadingBarManager.EmitSignal("IncreaseRegisteredTask");
        item1 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_1.tscn");
        item2 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_2.tscn");
        item3 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_3.tscn");
        item4 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_4.tscn");
        _httpRequestForGettingAllExhibits = new HttpRequest();
        _httpRequestForGettingAllDisplayArtifacts = new HttpRequest();
        AddChild(_httpRequestForGettingAllExhibits);
        AddChild(_httpRequestForGettingAllDisplayArtifacts);
        _httpRequestForGettingAllExhibits.RequestCompleted += HttpRequestForGettingAllExhibitsOnRequestCompleted;
        _httpRequestForGettingAllDisplayArtifacts.RequestCompleted += HttpRequestForGettingAllDisplayArtifactsOnRequestCompleted;
        // _httpRequestForGettingAllDisplayArtifacts.Request(ApiAddress.MuseumApiPath + "GetAllDisplayArtifacts");
        await Task.Delay(1000);
        _displayArtifacts = MuseumReferenceManager.Instance.DisplayArtifactServices.GetAllArtifacts();
        var result = MuseumReferenceManager.Instance.ExhibitServices.GetAllExhibits();
        AfterGettingAllExhibits(result);
        // MuseumReferenceManager.Instance.ExhibitServices
        MuseumActions.OnClickItem += OnClickItem;
    }

    private void HttpRequestForGettingAllDisplayArtifactsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        _displayArtifacts = JsonSerializer.Deserialize<List<Artifact>>(jsonStr);
        _httpRequestForGettingAllExhibits.Request(ApiAddress.MuseumApiPath + "GetAllExhibits");
    }


    private void HttpRequestForGettingAllExhibitsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        var exhibits = JsonSerializer.Deserialize<List<Exhibit>>(jsonStr);
        AfterGettingAllExhibits(exhibits);
        //EmitSignal(LoadingBarManager.SignalName.IncreaseCompletedTask);
        //GD.Print($"Number of exhibits {exhibits.Count}");
    }

    private void AfterGettingAllExhibits(List<Exhibit> exhibits)
    {
        // GD.Print($"got exhibits {exhibits.Count}");
        _museumRunningDataContainer = ServiceRegistry.Resolve<MuseumRunningDataContainer>();
        _museumRunningDataContainer.Exhibits = exhibits;
        _loadingBarManager.EmitSignal("IncreaseCompletedTask");
        SpawnExhibits(exhibits);
    }

    private void SpawnExhibits(List<Exhibit> exhibits)
    {
        foreach (var exhibit in exhibits)
        {
            if (exhibit.ExhibitVariationName == "BasicExhibit1x1")
            {
                SpawnExhibit(exhibit, item1);
            }
            else if (exhibit.ExhibitVariationName == "MediumWoodenExhibitBasic")
            {
                SpawnExhibit(exhibit, item3);
            }
            else if (exhibit.ExhibitVariationName == "MediumWoodenExhibitBasic2")
            {
                SpawnExhibit(exhibit, item4);
            }
            else if (exhibit.ExhibitVariationName == "BasicExhibit4x4")
            {
                SpawnExhibit(exhibit, item2);
            }
        }
    }

    private void SpawnExhibit(Exhibit exhibit, PackedScene packedScene)
    {
        var instance = (Node)packedScene.Instantiate();
        instance.GetNode<ExhibitItem>(".").SpawnFromDatabase(exhibit, _displayArtifacts);
        _itemsParent.AddChild(instance);
        instance.GetNode<Node2D>(".").Position =
            GameManager.tileMap.MapToLocal(new Vector2I(exhibit.XPosition, exhibit.YPosition));
        // instance.GetNode<Node2D>(".").Position += new Vector2(0, 9);
        MuseumActions.OnItemUpdated?.Invoke();
    }

    private void OnClickItem(Item item, Exhibit exhibit)
    {
        //GD.Print($"Clicked {item.ExhibitVariationName} {item.Name}");
        _exhibitEditorUi.Visible = true;
        _exhibitEditorUi.ReInitialize(item, exhibit);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        MuseumActions.OnClickItem -= OnClickItem;
    }
}