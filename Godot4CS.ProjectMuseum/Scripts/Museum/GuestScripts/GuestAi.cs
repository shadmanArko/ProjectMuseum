using System;
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
    protected float availableMoney;
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

    private int _needsDecayInterval = 3;
    private int _countForDecayInterval= 0;

    private bool _executingADecision = false;
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
        if (_countForDecayInterval >= _needsDecayInterval)
        {
            hungerLevel += hungerDecayRate;
            thirstLevel += thirstDecayRate;
            bladderLevel += bladderDecayRate;
            chargeLevel += chargeDecayRate;
            energyLevel += energyDecayRate;
            interestInArtifactLevel += interestInArtifactDecayRate;
            entertainmentLevel += entertainmentDecayRate;
            // GD.Print($"{Name}: " +
            //          $"hunger: {hungerLevel}, thirst: {thirstLevel}, bladder: {bladderLevel}, charge:{chargeLevel}, " +
            //          $"energy: {energyLevel}, interest{interestInArtifactLevel}, entert: {entertainmentLevel}");
            
            _countForDecayInterval = 0;
            if (!_executingADecision)
            {
                CheckForFulfillingNeeds();
            }
        }

        _countForDecayInterval++;
    }

    private void CheckForFulfillingNeeds()
    {
        float highestValue = Math.Max(hungerLevel, Math.Max(thirstLevel, Math.Max(interestInArtifactLevel, Math.Max(bladderLevel, Math.Max(entertainmentLevel, Math.Max(chargeLevel, energyLevel))))));
        var tolerance = 0.01;
        var output = "";
        if (Math.Abs(highestValue - hungerLevel) < tolerance)
        {
            output = ("Find Food");
        }
        else if (Math.Abs(highestValue - thirstLevel) < tolerance)
        {
            output = ("Find Drink");
        }
        else if (Math.Abs(highestValue - interestInArtifactLevel) < tolerance)
        {
            output = ("Find Artifact");
        }
        else if (Math.Abs(highestValue - bladderLevel) < tolerance)
        {
            output = ("Find Washroom");
        }
        else if (Math.Abs(highestValue - entertainmentLevel) < tolerance)
        {
            output = ("Find Entertainment");
        }
        else if (Math.Abs(highestValue - chargeLevel) < tolerance)
        {
            output = ("Find Charge");
        }
        else if (Math.Abs(highestValue - energyLevel) < tolerance)
        {
            output = ("Find Energy");
        }
        GD.Print($"{Name}: {output}");
    }

    public void FillNeed(GuestNeedsEnum need, float amount)
    {
        switch(need)
        {
            case GuestNeedsEnum.Hunger:
                hungerLevel -= amount;
                break;
            case GuestNeedsEnum.Thirst:
                thirstLevel -= amount;
                break;
            case GuestNeedsEnum.InterestInArtifact:
                interestInArtifactLevel -= amount;
                break;
            case GuestNeedsEnum.Bladder:
                bladderLevel -= amount;
                break;
            case GuestNeedsEnum.Charge:
                chargeLevel -= amount;
                break;
            case GuestNeedsEnum.Energy:
                energyLevel -= amount;
                break;
            case GuestNeedsEnum.Entertainment:
                entertainmentLevel -= amount;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(need), need, null);
        }
    }
    public override void _ExitTree()
    {
        base._ExitTree();
        MuseumActions.OnClickGuestAi -= OnClickGuestAi;
        MuseumActions.OnTimeUpdated -= OnTimeUpdated;

    }
}