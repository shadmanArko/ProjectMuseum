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
    private PackedScene item3;
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

    private HttpRequest _httpRequestForGettingBalance;
    private HttpRequest _httpRequestForReducingBalance;
    public override void _Ready()
    {
        item1 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_1.tscn");
        item2 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_2.tscn");
        item3 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_3.tscn");
        Item.OnItemPlaced += UpdateUiOnItemPlaced;
        // museumMoneyTextField = GetNode<RichTextLabel>("Bottom Panel/MuseumMoney");
        GD.Print("ready from ui being called");
        if(ExhibitPlacementConditionDatas == null) GD.Print("Null exhibit data");
        _httpRequestForGettingBalance = new HttpRequest();
        _httpRequestForReducingBalance = new HttpRequest();
        AddChild(_httpRequestForGettingBalance);
        AddChild(_httpRequestForReducingBalance);
        string url = "http://localhost:5178/api/MuseumTile/GetMuseumBalance/museum0";
        _httpRequestForGettingBalance.Request(url);
        _httpRequestForGettingBalance.RequestCompleted += OnHttpRequestForGettingBalanceCompleted;
        _httpRequestForReducingBalance.RequestCompleted += OnHttpRequestCompletedForReducingBalance;
    }
    private void OnHttpRequestForGettingBalanceCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        GD.Print("getting balance " + jsonStr);
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
    public void OnExhibit1Pressed()
    {
        var instance = (Node)item3.Instantiate();
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
        string url = $"http://localhost:5178/api/MuseumTile/ReduceMuseumBalance/museum0/{itemPrice}";
        _httpRequestForReducingBalance.Request(url);
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