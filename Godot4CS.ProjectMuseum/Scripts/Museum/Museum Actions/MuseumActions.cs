using System;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.ProfessorScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class MuseumActions : Node
{
    public static Action<Item, Exhibit> OnClickItem;
    public static Action<string> OnClickWallForUpdatingWallPaper;
    public static Action OnWallpaperSuccessfullyUpdated;
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
    public static Action<Texture2D> OnPreviewWallpaperUpdated;

    public static Action<bool> OnClickMuseumGateToggle;
    public static Action OnClickCloseTownUi;
    public static Action<int> TotalGuestsUpdated;
    #region TimeActions

    public static Action OnTownMapButtonClicked;

    #endregion
    #region TimeActions

    public static Action<int, int, int, int, int> OnTimeUpdated;
    public static Action OnClickPausePlayButton;
    public static Action<bool> OnTimePauseValueUpdated;
    public static Action<int> OnClickTimeSpeedButton;

    #endregion
    #region PlayerActions

    public static Action<Vector2I> PlayerEnteredNewTile;
    public static Action OnPlayerSavedGame;
    public static Action OnPlayerSleepAndSavedGame;
    public static Action<PathNavigatorCharacter> PathEnded;

    #endregion
    #region PopUpUi

    public static Action OnClickYesOfPopUpUi;
    public static Action OnClickNoOfPopUpUi;
    public static Action<string> OnNeedOfPopUpUi;

    #endregion
    #region Story

    public static Action<int> PlayStoryScene;
    public static Action<int> StorySceneEnded;
    public static Action<string> StorySceneEntryEnded;
    public static Action<string> TutorialSceneEntryEnded;
    public static Action<int> PlayTutorial;
    public static Action<string> OnTutorialUpdated;
    public static Action OnTutorialEnded;
    public static Action<string> OnPlayerPerformedTutorialRequiringAction;

    #endregion
}