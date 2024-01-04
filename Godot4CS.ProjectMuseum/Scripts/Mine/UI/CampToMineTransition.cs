using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class CampToMineTransition : Button
{
    private PlayerControllerVariables _playerControllerVariables;
    private CampExitPromptUi _campExitPromptUi;

    private AutoAnimationController _autoAnimationController;
    
    public override void _Ready()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _autoAnimationController = ReferenceStorage.Instance.AutoAnimationController;
        _campExitPromptUi = ReferenceStorage.Instance.CampExitPromptUi;
        // _campExitPromptUi.SleepForTheNightButton.ButtonUp += TransitFromCampToMine;
        _campExitPromptUi.SleepForTheNightButton.ButtonUp += DeactivateCampExitPromptUi;

        // _campExitPromptUi.ReturnToMuseumButton.ButtonUp += TransitFromCampToMuseum;
        _campExitPromptUi.ReturnToMuseumButton.ButtonUp += DeactivateCampExitPromptUi;
    }
    
    private async void TransitFromCampToMine()
    {
        GD.PrintErr("TransitFromCampToMine");
        var sceneTransition = ReferenceStorage.Instance.SceneTransition;
        await sceneTransition.FadeIn();
        await Task.Delay(2000);
        await sceneTransition.FadeOut();
        ReferenceStorage.Instance.MineTimeSystem.StartNextDayMineExcavation();
        _autoAnimationController._Ready();
    }

    private async void TransitFromCampToMuseum()
    {
        DeactivateCampExitPromptUi();
        var sceneTransition = ReferenceStorage.Instance.SceneTransition;
        await sceneTransition.FadeIn();
        _playerControllerVariables.CanMove = false;
        await Task.Delay(2000);
    }
    
    private void DeactivateCampExitPromptUi()
    {
        _campExitPromptUi.Visible = false;
    }
}