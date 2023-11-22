using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum;

public partial class ExhibitController : Node2D
{
    [Export] private ExhibitEditorUi _exhibitEditorUi;
    private HttpRequest _httpRequestForGettingAllExhibits;
    private PackedScene item1;
    public override void _Ready()
    {
        item1 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_1.tscn");
        _httpRequestForGettingAllExhibits = new HttpRequest();
        AddChild(_httpRequestForGettingAllExhibits);
        _httpRequestForGettingAllExhibits.RequestCompleted += HttpRequestForGettingAllExhibitsOnRequestCompleted;
        _httpRequestForGettingAllExhibits.Request(ApiAddress.MuseumApiPath + "GetAllExhibits");
        MuseumActions.OnClickItem += OnClickItem;
    }

    private void HttpRequestForGettingAllExhibitsOnRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        var exhibits = JsonSerializer.Deserialize<List<Exhibit>>(jsonStr);
        foreach (var exhibit in exhibits)
        {
            var instance = (Node)item1.Instantiate();
            AddChild(instance);
            instance.GetNode<Node2D>(".").Position =
                GameManager.TileMap.MapToLocal(new Vector2I(exhibit.XPosition, exhibit.YPosition));
        }
        GD.Print($"Number of exhibits {exhibits.Count}");
    }

    private void OnClickItem(Item item)
    {
        GD.Print($"Clicked {item.ExhibitVariationName} {item.Name}");
        _exhibitEditorUi.Visible = true;
    }
}