using System;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class MuseumActions : Node
{
    public static Action<Item> OnClickItem;
    public static Action<Draggable> DragStarted;
    public static Action<Draggable> DragEnded;
    public static Action<BuilderCardType, string> OnClickBuilderCard;
    public static Action<float> OnMuseumBalanceUpdated;
    public static Action<float> OnMuseumBalanceReduced;
    public static Action<float> OnMuseumBalanceAdded;
    public static Action OnItemUpdated;
    public static Action<string> OnBottomPanelButtonClicked;
}