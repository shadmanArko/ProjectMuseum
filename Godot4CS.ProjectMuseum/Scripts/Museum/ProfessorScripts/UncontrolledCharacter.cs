using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using Godot4CS.ProjectMuseum.Tests.DragAndDrop;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.ProfessorScripts;

public partial class UncontrolledCharacter : PathNavigatorCharacter
{
    [Export] private Array<Vector2I> _professorEnteringCoordinates;
    [Export] private Vector2I _museumExitingCoordinate;
    private int _currentDirectionIndex = 0;
    [Export] private string _animationNameAfterInitialMovement;
    private bool _exitingMuseum = false;
    private Color _startColor;
    public override async void _Ready()
    {
        base._Ready();
        await Task.Delay(1000);
        _startColor = Modulate;
        _museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
        MuseumActions.PathEnded += PathEnded;
        MuseumActions.OnPlayerInteract += OnPlayerInteract;
    }

    private void OnPlayerInteract(Vector2I playerPosition, PlayerDirectionsEnum playerDirection)
    {
        GD.Print("player interact call");
        var myPosition = GameManager.TileMap.LocalToMap(Position);
        var myDirection = GetPlayerDirectionsEnum();

        GD.Print($"me {Name} player direction { playerDirection}, my direction {myDirection}, player pos {playerPosition}, my pos {myPosition}");
        if (Math.Abs(myPosition.X - playerPosition.X) <= 1 || Math.Abs(myPosition.Y - playerPosition.Y) <= 1)
        {
            if (myDirection == PlayerDirectionsEnum.FrontLeft && playerDirection == PlayerDirectionsEnum.BackRight)
            {
                MuseumActions.OnPlayerInteractWith?.Invoke(Name);
                if (Name == "Professor")
                {
                    MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("TalkToProfessor");
                }
            }
        }
    }

    public void StartFollowingDirection()
    {
        Visible = true;
        FollowDirections();
    }

    private void PathEnded(PathNavigatorCharacter obj)
    {
        
        GD.Print("Path ended call prom professor");
        if (obj == this)
        {
            if (_exitingMuseum)
            {
                Visible = false;
                return;
            }
            FollowDirections();
        }
    }

    private void FollowDirections()
    {
        if (_currentDirectionIndex < _professorEnteringCoordinates.Count)
        {
            SetPath(_professorEnteringCoordinates[_currentDirectionIndex]);
            _currentDirectionIndex++;
        }
        else
        {
            GD.Print("manual animation");
            _animationPlayerInstance.Play(_animationNameAfterInitialMovement);
        }
    }
    public void ExitMuseum()
    {
        _exitingMuseum = true;
        SetPath(_museumExitingCoordinate);
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        MuseumActions.PathEnded -= PathEnded;
        MuseumActions.OnPlayerInteract -= OnPlayerInteract;


    }

    private bool _mouseOnCharacter;
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_left_click") && _mouseOnCharacter)
        {
            GD.Print("Mouse Clicked"+ Name);
            Modulate = Colors.Brown;
            
        }
    }


    private void OnMouseEntered()
    {
        _mouseOnCharacter = true;
        Modulate = Colors.Burlywood;
        GD.Print("Mouse Entered"+ Name);
    }
    private void OnMouseExit()
    {
        _mouseOnCharacter = false;
        Modulate = _startColor;
        GD.Print("Mouse Exit" + Name);
    }
   
    
}