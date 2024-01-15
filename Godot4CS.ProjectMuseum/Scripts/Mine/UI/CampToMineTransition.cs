using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Player.Systems;

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
        _campExitPromptUi.SleepForTheNightButton.ButtonUp += DeactivateCampExitPromptUi;
        _campExitPromptUi.ReturnToMuseumButton.ButtonUp += DeactivateCampExitPromptUi;
        _campExitPromptUi.ReturnToMineButton.ButtonUp += DeactivateCampExitPromptUi;
    }
    
    private async void TransitFromCampToMineTheNextDay()
    {
        GD.PrintErr("TransitFromCampToMine");
        var sceneTransition = ReferenceStorage.Instance.SceneTransition;
        await sceneTransition.FadeIn();
        await Task.Delay(2000);
        await sceneTransition.FadeOut();

        if (ReferenceStorage.Instance.MineTimeSystem.GetTime().Days <= 5)
        {
            ReferenceStorage.Instance.MineTimeSystem.StartNextDayMineExcavation();
            HealthSystem.RestorePlayerFullHealth(200, _playerControllerVariables);
            EnergySystem.RestoreFullEnergy(200, _playerControllerVariables);
            _autoAnimationController._Ready();
        }
        else
            ReferenceStorage.Instance.MineTimeSystem.StartNextDayMineExcavation();
    }

    private void TransitionFromCampToMineOnTheSameDay()
    {
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