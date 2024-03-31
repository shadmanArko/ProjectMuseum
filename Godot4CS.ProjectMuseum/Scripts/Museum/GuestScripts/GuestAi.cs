using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Plugins.AStarPathFinding;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.HelperScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.GuestScripts;

public partial class GuestAi : CharacterBody2D
{
    protected float hungerLevel;
    protected float interestInArtifactLevel;
    protected float thirstLevel;
    protected float bladderLevel;
    protected float chargeLevel;
    protected float energyLevel;
    protected float entertainmentLevel;
    
    protected float hungerDecayRate;
    protected float interestInArtifactDecayRate;
    protected float thirstDecayRate;
    protected float bladderDecayRate;
    protected float chargeDecayRate;
    protected float energyDecayRate;
    protected float entertainmentDecayRate;
    
    //Guest Ai selection
    [Export] private Sprite2D _collisionShape2D;
    [Export] private Sprite2D _selectionIndicator;
    [Export] private Sprite2D _selectionIndicatorBottom;
    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustReleased("ui_left_click"))
        {
            
            if ( _collisionShape2D.GetRect().HasPoint(GetLocalMousePosition()))
            {
                // GD.Print($"Clicked on guest {Name}");
                MuseumActions.OnClickGuestAi?.Invoke(this);
            }
        }
        
    }
    private void OnClickGuestAi(GuestAi guestAi)
    {
        _selectionIndicator.Visible = guestAi == this;
        _selectionIndicatorBottom.Visible = guestAi == this;
    }
    public override void _EnterTree()
    {
        base._EnterTree();
        MuseumActions.OnClickGuestAi += OnClickGuestAi;
        MuseumActions.OnTimeUpdated += OnTimeUpdated;
    }

    private void OnTimeUpdated(int minutes, int hours, int days, int months, int years)
    {

        hungerLevel += hungerDecayRate;
        thirstLevel += thirstDecayRate;
        bladderLevel += bladderDecayRate;
        chargeLevel += chargeDecayRate;
        energyLevel += energyDecayRate;
        interestInArtifactLevel += interestInArtifactDecayRate;
        entertainmentLevel += entertainmentDecayRate;
        GD.Print($"{Name}: hunger: {hungerLevel}, thirst: {thirstLevel}, bladder: {bladderLevel}, charge:{chargeLevel}, energy: {energyLevel}, interest{interestInArtifactLevel}, entert: {entertainmentLevel}");
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        MuseumActions.OnClickGuestAi -= OnClickGuestAi;
        MuseumActions.OnTimeUpdated -= OnTimeUpdated;

    }
}