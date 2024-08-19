using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Mine.UI;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineEntranceController : Node2D
{
    private MineExitPromptUi _mineExitPromptUi;
    [Export] private CollisionShape2D _entranceBlockerCollisionShape;

    public override void _Ready()
    {
        _mineExitPromptUi = ReferenceStorage.Instance.MineExitPromptUi;
    }

    private void OnPlayerEnterTrigger(Node body)
    {
        if(body is not PlayerController player) return;
        ActivateMineExitPromptUi();
    }

    private void OnPlayerExitTrigger(Node body)
    {
        if(body is not PlayerController player) return;
        _entranceBlockerCollisionShape.SetDeferred("disabled", false);
        DeactivateMineExitPromptUi();
    }

    private void ActivateMineExitPromptUi()
    {
        _mineExitPromptUi.Visible = true;
    }

    private void DeactivateMineExitPromptUi()
    {
        _mineExitPromptUi.Visible = false;
    }

    private void OpenMineEntrance()
    {
        _entranceBlockerCollisionShape.SetDeferred("disabled", true);
    }

    private void CloseMineEntrance()
    {
        _entranceBlockerCollisionShape.SetDeferred("disabled", false);
    }
}