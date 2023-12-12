using System.Threading.Tasks;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class CampToMineTransition : Node
{
    private PlayerControllerVariables _playerControllerVariables;

    public override void _Ready()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
    }
    
    private void TransitFromCampToMine()
    {
        CampToMine();
    }

    private async void CampToMine()
    {
        var sceneTransition = ReferenceStorage.Instance.sceneTransition;
        await sceneTransition.FadeIn();
        _playerControllerVariables.CanMove = false;
        await Task.Delay(2000);
        _playerControllerVariables.State = MotionState.Grounded;
        _playerControllerVariables.IsAffectedByGravity = false;
        _playerControllerVariables.Player.Position = new Vector2(250,-60);
        await sceneTransition.FadeOut();
    }
    
}