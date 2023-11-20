using System;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public static class MuseumActions
{
    public static Action<Item> OnClickItem;
    public static Action<Draggable> DragStarted;
    public static Action<Draggable> DragEnded;
    public static Action<BuilderCardType, string> OnClickBuilderCard;
}