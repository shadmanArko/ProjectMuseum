using Godot;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

namespace Godot4CS.ProjectMuseum.Scripts.Museum;

public partial class MuseumBalanceController: Node2D
{
    public override void _Ready()
    {
        MuseumActions.OnMuseumBalanceAdded += OnMuseumBalanceAdded;
        MuseumActions.OnMuseumBalanceReduced += OnMuseumBalanceReduced;
        MuseumActions.OnMuseumBalanceUpdated += OnMuseumBalanceUpdated;
    }

    private void OnMuseumBalanceUpdated(float obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnMuseumBalanceReduced(float obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnMuseumBalanceAdded(float obj)
    {
        throw new System.NotImplementedException();
    }
}