using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.UI;

public partial class CampToMineTransition : Button
{
    private PlayerControllerVariables _playerControllerVariables;
    private CampExitPromptUi _campExitPromptUi;

    [Export] private Vector2 _p0;
    [Export] private Vector2 _p1;
    [Export] private Vector2 _p2;

    [Export] private double _time;
    private Vector2 _newPos = new(420,-60);
    
    public override void _Ready()
    {
        _playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
        _campExitPromptUi = ReferenceStorage.Instance.campExitPromptUi;
        _campExitPromptUi.SleepForTheNightButton.ButtonUp += TransitFromCampToMine;
        _campExitPromptUi.SleepForTheNightButton.ButtonUp += DeactivateCampExitPromptUi;

        _campExitPromptUi.ReturnToMuseumButton.ButtonUp += TransitFromCampToMuseum;
        _campExitPromptUi.ReturnToMuseumButton.ButtonUp += DeactivateCampExitPromptUi;
        SetProcess(false);
        SetPhysicsProcess(false);
    }
    
    private void TransitFromCampToMine()
    {
        _playerControllerVariables.Player.Position = new Vector2(250, -60);
        _playerControllerVariables.CanMove = false;
		
        _playerControllerVariables.Gravity = 0;
        _playerControllerVariables.State = MotionState.Grounded;
        SetProcess(true);
        SetPhysicsProcess(false);
    }

    private void TransitFromCampToMuseum()
    {
        
    }

    #region Process

    public override void _Process(double delta)
    {
        AutoMoveToPosition();
    }
    
    public override void _PhysicsProcess(double delta)
    {
        _playerControllerVariables.Player.Position = AutoJumpIntoMine((float) _time);
        _time += delta;

        if (!(_time >= 1.2f)) return;
        SetProcess(false);
        SetPhysicsProcess(false);
        _playerControllerVariables.CanMove = true;
        _playerControllerVariables.IsAffectedByGravity = true;
        _playerControllerVariables.Gravity = 25f;
    }

    #endregion
    
    #region Auto Animations

    private void AutoMoveToPosition()
    {
        if(_playerControllerVariables.Player.Position.X <= _newPos.X)
        {
            _playerControllerVariables.Player.animationController.Play("run");
            _playerControllerVariables.Player.Translate(new Vector2(0.02f,0));
        }
        else
        {
            _p0 = _playerControllerVariables.Player.Position;
            _p2 = _playerControllerVariables.Player.Position + new Vector2(60, 0);
            _p1 = new Vector2((_p0.X + _p2.X) / 2, _p0.Y - 75);

            SetProcess(false);
            SetPhysicsProcess(true);
        }
    }
	
    private Vector2 AutoJumpIntoMine(float t)
    {
        var q0 = _p0.Lerp(_p1, t);
        var q1 = _p1.Lerp(_p2, t);
        var r = q0.Lerp(q1, t);
        return r;
    }

    #endregion

    private void DeactivateCampExitPromptUi()
    {
        _campExitPromptUi.Visible = false;
    }
}