using System;
using System.Collections.Generic;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.GuestScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Model;
using Godot4CS.ProjectMuseum.Scripts.Museum.ProfessorScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class MuseumActions : Node
{
    public static Action<Item, Exhibit> OnClickItem;
    public static Action<Item, Shop> OnClickShopItem;
    public static Action<GuestAi> OnClickGuestAi;
    public static Action<GuestAi> OnGuestAiUpdated;
    public static Action<string> OnClickWallForUpdatingWallPaper;
    public static Action OnWallpaperSuccessfullyUpdated;
    public static Action<Draggable> DragStarted;
    public static Action<Draggable> DragEnded;
    public static Action<BuilderCardType, string> OnClickBuilderCard;
    public static Action OnMuseumTilesUpdated;
    public static Action<Node> OnGuestExitMuseum;
    public static Action<Node> OnGuestEnterMuseum;
    public static Action OnGuestExitScene;
    public static Action<float> OnMuseumBalanceUpdated;
    public static Action<float> OnMuseumBalanceReduced;
    public static Action<float> OnMuseumBalanceAdded;
    public static Action OnItemUpdated;
    public static Action OnItemPlaced;
    public static Action<Item, Vector2> OnItemPlacedOnTile;
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
    public static Action OnSleepComplete;

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
    public static Action<Vector2I, PlayerDirectionsEnum> OnPlayerInteract;
    public static Action<string> OnPlayerInteractWith;
    public static Action<PlayerInfo> OnPlayerGetPlayerInfo;
    public static Action OnPlayerClickedAnEmptyHouse;


    #endregion
    #region PopUpUi

    public static Action OnClickYesOfPopUpUi;
    public static Action OnClickNoOfPopUpUi;
    public static Action OnClickArtifactsLensButton;
    public static Action<WallHeightEnum> OnClickWallHeightChangeButton;
    public static Action<string> OnNeedOfPopUpUi;
    public static Action<string> OnNeedOfWarning;

    #endregion
    #region Story

    public static Action<int> PlayStoryScene;
    public static Action OnConceptStoryCompleted;
    public static Action<int> StorySceneEnded;
    public static Action<string> StorySceneEntryEnded;
    public static Action<string> StorySceneEntryStarted;
    public static Action<string> TutorialSceneEntryEnded;
    public static Action<int> PlayTutorial;
    public static Action<string> OnTutorialUpdated;
    public static Action OnTutorialEnded;
    public static Action<string> OnPlayerPerformedTutorialRequiringAction;

    #endregion
    #region ButtonActions

    public static Action OnClinkStartNewGameButton;


    #endregion
    #region MuseumZone

    public static Action<List<string>> OnSelectTilesForZone;
    public static Action OnNotSelectingEnoughTiles;
    public static Action<Color> OnZoneColorChanged;
    public static Action OnZoneCreationUiClosed;
    
    #endregion
    #region Exhibit Editor

    public static Action<List<RawArtifactDescriptive>> OnRawArtifactDescriptiveDataLoaded;
    public static Action<List<RawArtifactFunctional>> OnRawArtifactFunctionalDataLoaded;

    #endregion

    #region Shop

    public static Action<Product, string, float> OnProductReplaced;
    public static Action<Product, float> OnProductPriceUpdated;
    public static Action<List<Product>> OnGettingAllProducts;

    #endregion

    #region Expansion

    public static Action<Vector2I> OnCallForMuseumExpansion;
    public static Action OnMuseumExpanded;

    #endregion

    #region DayEnd

    public static Action DayEnded;
    public static Action<MuseumDayEndReport> DayEndReportGenerated;

    #endregion
}
public enum WallHeightEnum
{
    Original,
    Mid,
    Small
}