using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.ProfessorScripts;

public partial class UncontrolledCharacter : PathNavigatorCharacter
{
    [Export] private Array<Vector2I> _professorEnteringCoordinates;
    [Export] private Vector2I _museumExitingCoordinate;
    private int _currentDirectionIndex = 0;
    [Export] private string _animationNameAfterInitialMovement;
    private bool _exitingMuseum = false;
    public override async void _Ready()
    {
        base._Ready();
        await Task.Delay(1000);
   
        _museumTileContainer = ServiceRegistry.Resolve<MuseumTileContainer>();
        MuseumActions.PathEnded += PathEnded;
    }

    public void StartFollowingDirection()
    {
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

    }
}