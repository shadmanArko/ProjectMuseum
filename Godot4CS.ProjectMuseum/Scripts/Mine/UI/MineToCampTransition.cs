using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class MineToCampTransition : Button
{
    private PlayerControllerVariables _playerControllerVariables;
    private MineExitPromptUi _mineExitPromptUi;
    private CampExitPromptUi _campExitPromptUi;
    
    public override void _Ready()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _mineExitPromptUi = ReferenceStorage.Instance.MineExitPromptUi;
        _campExitPromptUi = ReferenceStorage.Instance.CampExitPromptUi;
        _mineExitPromptUi.ReturnToCampButton.ButtonUp += TransitFromMineToCamp;
    }

    private void TransitFromMineToCamp()
    {
        ReferenceStorage.Instance.MineTimeSystem.PauseTimer();
        MineToCamp();
    }

    private async void MineToCamp()
    {
        DeactivateMineExitPromptUi();
        var sceneTransition = ReferenceStorage.Instance.SceneTransition;
        await sceneTransition.FadeIn();
        _playerControllerVariables.CanMove = false;
        await Task.Delay(2000);
        _playerControllerVariables.State = MotionState.Grounded;
        _playerControllerVariables.IsAffectedByGravity = false;
        _playerControllerVariables.Player.Position = new Vector2(250,-60);
        await sceneTransition.FadeOut();
        
        ActivateCampExitPromptUi();
    }
    
    private void ActivateMineExitPromptUi()
    {
        _mineExitPromptUi.Visible = true;
    }

    private void DeactivateMineExitPromptUi()
    {
        _mineExitPromptUi.Visible = false;
    }
    
    private void ActivateCampExitPromptUi()
    {
        _campExitPromptUi.Visible = true;
    }

    private void DeactivateCampExitPromptUi()
    {
        _campExitPromptUi.Visible = false;
    }
}