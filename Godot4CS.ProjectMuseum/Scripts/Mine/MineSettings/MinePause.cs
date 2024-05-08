using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MineSettings;

public partial class MinePause : Node2D
{
    public bool IsPaused { get; set; }


    public override void _Ready()
    {
        SubscribeToActions();
    }

    private void SubscribeToActions()
    {
        MineActions.OnGamePaused += Pause;
        MineActions.OnGameUnpaused += Unpause;
    }
    private void UnsubscribeToActions()
    {
        MineActions.OnGamePaused -= Pause;
        MineActions.OnGameUnpaused -= Unpause;
    }
    
    private void Pause()
    {
        GetTree().Paused = true;
        IsPaused = true;
    }
    
    public override void _Input(InputEvent inputEvent)
    {
        if (!inputEvent.IsActionReleased("Inventory")) return;
        if (ReferenceStorage.Instance.MinePause.IsPaused)
        {
            GD.Print("UNPAUSING");
            MineActions.OnGameUnpaused?.Invoke();
            ReferenceStorage.Instance.InventoryViewController.HideInventory();
        }
        else
        {
            GD.Print("PAUSING");
            MineActions.OnGamePaused?.Invoke();
            ReferenceStorage.Instance.InventoryViewController.ShowInventory();
        }
    }


    private void Unpause()
    {
        GetTree().Paused = false;
        IsPaused = false;
    }

    public override void _ExitTree()
    {
        UnsubscribeToActions();
    }
}