using System;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class MuseumActions : Node
{
    public static Action<Item, Exhibit> OnClickItem;
    public static Action<Draggable> DragStarted;
    public static Action<Draggable> DragEnded;
    public static Action<BuilderCardType, string> OnClickBuilderCard;
    public static Action<float> OnMuseumBalanceUpdated;
    public static Action<float> OnMuseumBalanceReduced;
    public static Action<float> OnMuseumBalanceAdded;
    public static Action OnItemUpdated;
    public static Action<string> OnBottomPanelButtonClicked;
    public static Action<BuilderCardType> OnBottomPanelBuilderCardToggleClicked;
    public static Action<Artifact, int> ArtifactDroppedOnSlot;
    public static Action<Artifact, int> ArtifactRemovedFromSlot;
    public static Action<Artifact, Item, int> ArtifactDroppedOnExhibitSlot;
    public static Action<Artifact, Item, int> ArtifactRemovedFromExhibitSlot;

    public static Action<bool> OnClickMuseumGateToggle;
    public static Action<int> TotalGuestsUpdated;

    #region TimeActions

    public static Action<int, int, int, int, int> OnTimeUpdated;
    public static Action OnClickPausePlayButton;
    public static Action<int> OnClickTimeSpeedButton;

    #endregion
    #region PlayerActions

    public static Action<Vector2I> PlayerEnteredNewTile;

    #endregion
    #region PopUpUi

    public static Action OnClickYesOfPopUpUi;
    public static Action OnClickNoOfPopUpUi;
    public static Action<string> OnNeedOfPopUpUi;

    #endregion
    #region Story

    public static Action<int> PlayStoryScene;
    public static Action<int> StorySceneEnded;

    #endregion
}