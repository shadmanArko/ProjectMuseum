using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot.DependencyInjection.Attributes;
using ProjectMuseum.Models;
using Godot.DependencyInjection.Services.Input;

public partial class MuseumUi : Control  // Replace with the appropriate node type for your UI
{
    private PackedScene item1;
    private PackedScene item2;
    [Export] private RichTextLabel museumMoneyTextField;
    [Export]public Node2D ItemsParent;
    [Inject]
    public List<ExhibitPlacementConditionData> ExhibitPlacementConditionDatas { get; set; }
    
    [Inject]
    public void Inject(List<ExhibitPlacementConditionData> exhibitPlacementConditionDatas)
    {
        ExhibitPlacementConditionDatas = exhibitPlacementConditionDatas;
        GD.Print("inject being called");
    }
    public override void _Ready()
    {
        item1 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_1.tscn");
        item2 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_2.tscn");
        Item.OnItemPlaced += UpdateUiOnItemPlaced;
        // museumMoneyTextField = GetNode<RichTextLabel>("Bottom Panel/MuseumMoney");
        GD.Print("ready from ui being called");
        if(ExhibitPlacementConditionDatas == null) GD.Print("Null exhibit data");
        
        HttpRequest http = GetNode<HttpRequest>("HTTPRequest");
        string url = "http://localhost:5178/api/MuseumTile/GetMuseumBalance/museum0";
        http.Request(url);
    }
    private void OnHttpRequestCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        var museumBalance = JsonSerializer.Deserialize<float>(jsonStr);
        UpdateMuseumBalanceText(museumBalance.ToString("0.00"));
    }

    private void UpdateMuseumBalanceText(string jsonStr)
    {
        museumMoneyTextField.Text = $"${jsonStr}";
    }

    public void OnExhibit0Pressed()
    {
        var instance = (Node)item1.Instantiate();
        // GetTree().Root.AddChild(instance);
        ItemsParent.AddChild(instance);
        var scriptInstance = instance.GetNode("." /* Replace with the actual path to the script node */);

        if (scriptInstance != null)
        {
            // Now you can access properties or call methods on the script instance
            scriptInstance.Set("selectedItem", true);
        }
        else
        {
            GD.Print("Item script not found");
        }
    }

    public void OnExhibit3Pressed()
    {
        var instance = (Node)item2.Instantiate();
        // GetTree().Root.AddChild(instance);
        ItemsParent.AddChild(instance);
        var scriptInstance = instance.GetNode("." /* Replace with the actual path to the script node */);

        if (scriptInstance != null)
        {
            // Now you can access properties or call methods on the script instance
            scriptInstance.Set("selectedItem", true);
        }
        else
        {
            GD.Print("Item script not found");
        }
    }

    void UpdateUiOnItemPlaced(float itemPrice)
    {
        GD.Print($"Item Placed of price {itemPrice}");
        
        HttpRequest http1 = GetNode<HttpRequest>("HTTPRequest");
        http1.RequestCompleted += OnHttpRequestCompletedForReducingBalance;
        string url = $"http://localhost:5178/api/MuseumTile/ReduceMuseumBalance/museum0/{itemPrice}";
        http1.Request(url);
    }

    private void OnHttpRequestCompletedForReducingBalance(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
         var museum = JsonSerializer.Deserialize<ProjectMuseum.Models.Museum>(jsonStr);
        UpdateMuseumBalanceText(museum.Money.ToString("0.00"));
    }


    public override void _ExitTree()
    {
        Item.OnItemPlaced -= UpdateUiOnItemPlaced;
        base._ExitTree();
    }
}