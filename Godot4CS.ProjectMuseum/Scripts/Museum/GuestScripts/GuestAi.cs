using System;
using System.Collections.Generic;
using System.Security.AccessControl;
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
    public float availableMoney;
    public float hungerLevel;
    public float interestInArtifactLevel;
    public float thirstLevel;
    public float bladderLevel;
    public float chargeLevel;
    public float energyLevel;
    public float entertainmentLevel;
    public List<string> interestedInTags = new List<string>();
    public bool insideMuseum;
    protected float hungerDecayRate;
    protected float interestInArtifactDecayRate;
    protected float thirstDecayRate;
    protected float bladderDecayRate;
    protected float chargeDecayRate;
    protected float energyDecayRate;
    protected float entertainmentDecayRate;

    private int _needsDecayInterval = 1;
    private int _countForDecayInterval= 0;

    private bool _executingADecision = false;
    //Guest Ai selection
    [Export] private Sprite2D _collisionShape2D;
    [Export] private Sprite2D _selectionIndicator;
    [Export] private Sprite2D _selectionIndicatorBottom;
    [Export] private AnimationPlayer _selectionIndicatorAnimationPlayer;
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
        _selectionIndicatorAnimationPlayer.Play("selected");
    }
    public override void _EnterTree()
    {
        base._EnterTree();
        MuseumActions.OnClickGuestAi += OnClickGuestAi;
        MuseumActions.OnTimeUpdated += OnTimeUpdated;
    }

    private void OnTimeUpdated(int minutes, int hours, int days, int months, int years)
    {
        if (!insideMuseum) return;
        
        _countForDecayInterval++;
        if ( _countForDecayInterval >= _needsDecayInterval)
        {
            hungerLevel = Math.Clamp(hungerLevel + hungerDecayRate, -100, 100);
            thirstLevel = Math.Clamp(thirstLevel + hungerDecayRate, -100, 100);
            bladderLevel = Math.Clamp(bladderLevel + hungerDecayRate, -100, 100);
            chargeLevel = Math.Clamp(chargeLevel + hungerDecayRate, -100, 100);
            energyLevel = Math.Clamp(energyLevel + hungerDecayRate, -100, 100);
            interestInArtifactLevel = Math.Clamp(interestInArtifactLevel + hungerDecayRate, -100, 100);
            entertainmentLevel = Math.Clamp(entertainmentLevel + hungerDecayRate, -100, 100);
            // GD.Print($"{Name}: " +
            //          $"hunger: {hungerLevel}, thirst: {thirstLevel}, bladder: {bladderLevel}, charge:{chargeLevel}, " +
            //          $"energy: {energyLevel}, interest{interestInArtifactLevel}, entert: {entertainmentLevel}");
            
            _countForDecayInterval = 0;
            if (!_executingADecision)
            {
                // CheckForNeedsToFulfill();
            }
        }

        
    }

    float NegativeLinear(float x)
    {
        return -x;
    }
    protected GuestNeedsEnum CheckForNeedsToFulfill()
    {
        var hungerModifier = NegativeLinear(hungerLevel);
        var thirstModifier = NegativeLinear(thirstLevel);
        var interestInArtifactModifier = NegativeLinear(interestInArtifactLevel);
        var bladderModifier = NegativeLinear(bladderLevel);
        var entertainmentModifier = NegativeLinear(entertainmentLevel);
        var chargeModifier = NegativeLinear(chargeLevel);
        var energyModifier = NegativeLinear(energyLevel);
        GuestNeedsEnum guestNeed = GuestNeedsEnum.InterestInArtifact;
        float value = Math.Max(hungerModifier, Math.Max(thirstModifier, Math.Max(interestInArtifactModifier, Math.Max(bladderModifier, Math.Max(entertainmentModifier, Math.Max(chargeModifier, energyModifier))))));
        var tolerance = 0.01;
        var output = "";
        if (Math.Abs(value - hungerModifier) < tolerance)
        {
            output = ("Find Food");
            guestNeed = GuestNeedsEnum.Hunger;
        }
        else if (Math.Abs(value - thirstModifier) < tolerance)
        {
            output = ("Find Drink");
            guestNeed = GuestNeedsEnum.Thirst;
        }
        else if (Math.Abs(value - interestInArtifactModifier) < tolerance)
        {
            output = ("Find Artifact");
            guestNeed = GuestNeedsEnum.InterestInArtifact;
        }
        else if (Math.Abs(value - bladderModifier) < tolerance)
        {
            output = ("Find Washroom");
            guestNeed = GuestNeedsEnum.Bladder;
        }
        else if (Math.Abs(value - entertainmentModifier) < tolerance)
        {
            output = ("Find Entertainment");
            guestNeed = GuestNeedsEnum.Entertainment;
        }
        else if (Math.Abs(value - chargeModifier) < tolerance)
        {
            output = ("Find Charge");
            guestNeed = GuestNeedsEnum.Charge;
        }
        else if (Math.Abs(value - energyModifier) < tolerance)
        {
            output = ("Find Energy");
            guestNeed = GuestNeedsEnum.Energy;
        }
        GD.Print($"{Name}: {output}");
        return guestNeed;
    }

    public void FillNeed(GuestNeedsEnum need, float amount)
    {
        switch(need)
        {
            case GuestNeedsEnum.Hunger:
                hungerLevel = Math.Clamp(hungerLevel - amount, -100, 100);
                break;
            case GuestNeedsEnum.Thirst:
                thirstLevel = Math.Clamp(thirstLevel - amount, -100, 100);
                break;
            case GuestNeedsEnum.InterestInArtifact:
                interestInArtifactLevel = Math.Clamp(interestInArtifactLevel - amount, -100, 100);
                break;
            case GuestNeedsEnum.Bladder:
                bladderLevel = Math.Clamp(bladderLevel - amount, -100, 100);
                break;
            case GuestNeedsEnum.Charge:
                chargeLevel = Math.Clamp(chargeLevel - amount, -100, 100);
                break;
            case GuestNeedsEnum.Energy:
                energyLevel = Math.Clamp(energyLevel - amount, -100, 100);
                break;
            case GuestNeedsEnum.Entertainment:
                entertainmentLevel = Math.Clamp(entertainmentLevel - amount, -100, 100);
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