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
    private PackedScene _decorationItem;
    [Export] private RichTextLabel museumMoneyTextField;
    [Export] private Button _diggingPermitsButton;
    [Export] private Button _townMapButton;
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
    
    public override void _Ready()
    {
        item1 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_1.tscn");
        item2 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_2.tscn");
        item3 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_3.tscn");
        item4 = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Sub Scenes/exhibitItemNode_4.tscn");
        _decorationItem = (PackedScene)ResourceLoader.Load("res://Scenes/Museum/Decorations/decorationItem.tscn");
        Item.OnItemPlaced += UpdateUiOnItemPlaced;
        // museumMoneyTextField = GetNode<RichTextLabel>("Bottom Panel/MuseumMoney");
        GD.Print("ready from ui being called");
        if(ExhibitPlacementConditionDatas == null) GD.Print("Null exhibit data");
        
       
        
        _diggingPermitsButton.Pressed += DiggingPermitsButtonOnPressed;
        _townMapButton.Pressed += TownMapButtonOnPressed;
        _museumGateCheckButton.Pressed += MuseumGateCheckButtonOnPressed;
        MuseumActions.OnClickBuilderCard += OnClickBuilderCard;
    }

    private void TownMapButtonOnPressed()
    {
        MuseumActions.OnTownMapButtonClicked?.Invoke();
        MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("ClickedTownMap");
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
            HandleExhibitItemPlacement(cardName);
        }else if (builderCardType == BuilderCardType.Decoration)
        {
            HandleDecorationCardPlacement(builderCardType, cardName, _decorationItem);
        }
    }
    

    private void HandleDecorationCardPlacement(BuilderCardType builderCardType, string cardName, PackedScene decorationPackedScene)
    {
        GD.Print("Handling decoration placement");
        var instance = (Node)decorationPackedScene.Instantiate();
        Texture2D texture2D = GD.Load<Texture2D>($"res://Assets/2D/Sprites/{builderCardType}s/{cardName}.png");
        var sprite = instance.GetNode<Sprite2D>(".") ;
        sprite.Texture = texture2D;
        ItemsParent.AddChild(instance);
        if (_lastSelectedItem != null && IsInstanceValid(_lastSelectedItem) && _lastSelectedItem.selectedItem)
        {
            _lastSelectedItem.QueueFree();
        }
        var scriptInstance = instance.GetNode<DecorationItem>(".");
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

    private void HandleExhibitItemPlacement(string cardName)
    {
        if (cardName == "SmallWoodenExhibitBasic")
        {
            OnExhibit0Pressed();
        }
        else if (cardName == "MediumWoodenExhibitBasic")
        {
            OnExhibit1Pressed();
        }
        else if (cardName == "LargeWoodenExhibitBasic")
        {
            OnExhibit3Pressed();
        }
        else if (cardName == "MediumWoodenExhibitBasic2")
        {
            OnExhibit4Pressed();
        }
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
        // GD.Print($"Item Placed of price {itemPrice}");
        // string url = $"{ApiAddress.MuseumApiPath}ReduceMuseumBalance/museum0/{itemPrice}";
        // _httpRequestForReducingBalance.Request(url);
    }
    

    public override void _ExitTree()
    {
        Item.OnItemPlaced -= UpdateUiOnItemPlaced;
        _diggingPermitsButton.Pressed -= DiggingPermitsButtonOnPressed;
        _townMapButton.Pressed -= TownMapButtonOnPressed;
        MuseumActions.OnClickBuilderCard -= OnClickBuilderCard;
        _museumGateCheckButton.Pressed -= MuseumGateCheckButtonOnPressed;
        base._ExitTree();
    }
}