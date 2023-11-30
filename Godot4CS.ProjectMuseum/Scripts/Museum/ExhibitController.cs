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
    [Export] private Node2D _itemsParent;
    private HttpRequest _httpRequestForGettingAllExhibits;
    private PackedScene item1;
    private PackedScene item2;
    private PackedScene item3;
    private PackedScene item4;
    public override void _Ready()
    {
        item1 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_1.tscn");
        item2 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_2.tscn");
        item3 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_3.tscn");
        item4 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_4.tscn");
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
            if (exhibit.ExhibitVariationName == "SmallWoodenExhibitBasic")
            {
                SpawnExhibit(exhibit, item1);
            }else if (exhibit.ExhibitVariationName =="MediumWoodenExhibitBasic")
            {
                SpawnExhibit(exhibit, item3);

            }else if (exhibit.ExhibitVariationName =="MediumWoodenExhibitBasic2")
            {
                SpawnExhibit(exhibit, item4);

            }else if (exhibit.ExhibitVariationName =="LargeWoodenExhibitBasic")
            {
                SpawnExhibit(exhibit, item2);
            }

            
        }
        GD.Print($"Number of exhibits {exhibits.Count}");
    }

    private void SpawnExhibit(Exhibit exhibit, PackedScene packedScene)
    {
        var instance = (Node)packedScene.Instantiate();
        instance.GetNode<Item>(".").ExhibitData = exhibit;
        _itemsParent.AddChild(instance);
        instance.GetNode<Node2D>(".").Position =
            GameManager.TileMap.MapToLocal(new Vector2I(exhibit.XPosition, exhibit.YPosition));
        MuseumActions.OnItemUpdated?.Invoke();
    }

    private void OnClickItem(Item item, Exhibit exhibit)
    {
        GD.Print($"Clicked {item.ExhibitVariationName} {item.Name}");
        _exhibitEditorUi.Visible = true;
        _exhibitEditorUi.ReInitialize(item, exhibit);
    }
}