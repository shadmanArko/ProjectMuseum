using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Loading_Bar;

public partial class LoadingBarManager : Node
{
    public int registeredTask = 0;
    public int completedTask = 0;
    [Export] private ProgressBar _progressBar;
    [Export] private CanvasLayer _canvasLayer;

    public double TaskCompleted = 0;

    [Signal]
    public delegate void IncreaseRegisteredTaskEventHandler();
    
    [Signal]
    public delegate void IncreaseCompletedTaskEventHandler();

    public override void _Ready()
    {
        _canvasLayer.Visible = true;
        IncreaseRegisteredTask += RegisteredTaskIncrease;
        IncreaseCompletedTask += CompletedTaskIncrease;
        GD.Print("Loading Manager");
    }


    private void RegisteredTaskIncrease()
    {
        registeredTask++;
        TaskCompleted = (double)completedTask / registeredTask * 100.0;
        GD.Print(registeredTask);
        GD.Print(TaskCompleted);
    }

    private void CompletedTaskIncrease()
    {
        completedTask++;
        TaskCompleted = (double)completedTask / registeredTask * 100.0;
        GD.Print(TaskCompleted);
        _progressBar.Value = TaskCompleted;
        if (TaskCompleted >= 100)
        {
            _canvasLayer.Visible = false;
        }
        
    }

}