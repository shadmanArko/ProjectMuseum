using Godot;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Godot.DependencyInjection.Attributes;
using Godot4CS.ProjectMuseum.Scripts.Museum;
using Godot4CS.ProjectMuseum.Scripts.Museum.DragAndDrop;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Scripts.StaticClasses;
using ProjectMuseum.Models;

public partial class MuseumUi : Control  // Replace with the appropriate node type for your UI
{
    private PackedScene item1;
    private PackedScene item2;
    private PackedScene item3;
    private PackedScene item4;
    [Export] private RichTextLabel museumMoneyTextField;
    [Export] private Button _diggingPermitsButton;
    [Export] private CheckButton _museumGateCheckButton;
    [Export] private Control _diggingPermitsUi;
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
        item4 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_4.tscn");
        Item.OnItemPlaced += UpdateUiOnItemPlaced;
        // museumMoneyTextField = GetNode<RichTextLabel>("Bottom Panel/MuseumMoney");
        GD.Print("ready from ui being called");
        if(ExhibitPlacementConditionDatas == null) GD.Print("Null exhibit data");
        _httpRequestForGettingBalance = new HttpRequest();
        _httpRequestForReducingBalance = new HttpRequest();
        AddChild(_httpRequestForGettingBalance);
        AddChild(_httpRequestForReducingBalance);
        string url = $"{ApiAddress.UrlPrefix}Museum/GetMuseumBalance/museum0";
        _httpRequestForGettingBalance.Request(url);
        _httpRequestForGettingBalance.RequestCompleted += OnHttpRequestForGettingBalanceCompleted;
        _httpRequestForReducingBalance.RequestCompleted += OnHttpRequestCompletedForReducingBalance;
        _diggingPermitsButton.Pressed += DiggingPermitsButtonOnPressed;
        _museumGateCheckButton.Pressed += MuseumGateCheckButtonOnPressed;
        MuseumActions.OnClickBuilderCard += OnClickBuilderCard;
    }

    private void MuseumGateCheckButtonOnPressed()
    {
        MuseumActions.OnClickMuseumGateToggle.Invoke(_museumGateCheckButton.ButtonPressed);  
    }

    private void DiggingPermitsButtonOnPressed()
    {
        _diggingPermitsUi.Visible = true;
    }

    private string _cardName = "";
    private void OnClickBuilderCard(BuilderCardType builderCardType, string cardName)
    {
        _cardName = cardName;
        if ((builderCardType == BuilderCardType.Exhibit))
        {
            if (cardName == "SmallWoodenExhibitBasic")
            {
                OnExhibit0Pressed();
            }else if (cardName =="MediumWoodenExhibitBasic")
            {
                OnExhibit1Pressed();

            }else if (cardName =="LargeWoodenExhibitBasic")
            {
                OnExhibit3Pressed();
            }else if (cardName =="MediumWoodenExhibitBasic2")
            {
                OnExhibit4Pressed();
            }
        }
    }

    private void OnHttpRequestForGettingBalanceCompleted(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
        GD.Print("getting balance " + jsonStr);
        var museumBalance = JsonSerializer.Deserialize<float>(jsonStr);
        MuseumActions.OnMuseumBalanceUpdated?.Invoke(museumBalance);
        UpdateMuseumBalanceText(museumBalance.ToString("0.00"));
    }

    private void UpdateMuseumBalanceText(string jsonStr)
    {
        museumMoneyTextField.Text = $"${jsonStr}";
    }

    public void OnExhibit0Pressed()
    {
        SetUpItem(item1);
    }
    public void OnExhibit1Pressed()
    {
        SetUpItem(item3);
    }

    private Item _lastSelectedItem;
    private void SetUpItem(PackedScene packedScene)
    {
        var instance = (Node)packedScene.Instantiate();
        ItemsParent.AddChild(instance);
        if (_lastSelectedItem != null && IsInstanceValid(_lastSelectedItem) && _lastSelectedItem.selectedItem)
        {
            _lastSelectedItem.QueueFree();
        }
        var scriptInstance = instance.GetNode<ExhibitItem>(".");
        if (scriptInstance != null)
        {
            scriptInstance.Position = GetGlobalMousePosition();
            scriptInstance.Initialize(_cardName);
            _lastSelectedItem = scriptInstance;
        }
        else
        {
            GD.Print("Item script not found");
        }
    }
    

    public void OnExhibit3Pressed()
    {
        SetUpItem(item2);
    }
    public void OnExhibit4Pressed()
    {
        SetUpItem(item4);
    }

    void UpdateUiOnItemPlaced(float itemPrice)
    {
        GD.Print($"Item Placed of price {itemPrice}");
        string url = $"{ApiAddress.MuseumApiPath}ReduceMuseumBalance/museum0/{itemPrice}";
        _httpRequestForReducingBalance.Request(url);
    }

    private void OnHttpRequestCompletedForReducingBalance(long result, long responsecode, string[] headers, byte[] body)
    {
        string jsonStr = Encoding.UTF8.GetString(body);
         var museum = JsonSerializer.Deserialize<ProjectMuseum.Models.Museum>(jsonStr);
         MuseumActions.OnMuseumBalanceUpdated?.Invoke(museum.Money);
        UpdateMuseumBalanceText(museum.Money.ToString("0.00"));
    }


    public override void _ExitTree()
    {
        Item.OnItemPlaced -= UpdateUiOnItemPlaced;
        _httpRequestForGettingBalance.RequestCompleted -= OnHttpRequestForGettingBalanceCompleted;
        _httpRequestForReducingBalance.RequestCompleted -= OnHttpRequestCompletedForReducingBalance;
        _diggingPermitsButton.Pressed -= DiggingPermitsButtonOnPressed;
        MuseumActions.OnClickBuilderCard -= OnClickBuilderCard;
        _museumGateCheckButton.Pressed -= MuseumGateCheckButtonOnPressed;
        base._ExitTree();
    }
}