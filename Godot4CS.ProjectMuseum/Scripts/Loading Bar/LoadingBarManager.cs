using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Loading_Bar;

public partial class LoadingBarManager : Node
{
    public int registeredTask = 0;
    public int completedTask = 0;
    [Export] private HSlider _hSlider;

    public double TaskCompleted = 0;

    [Signal]
    public delegate void IncreaseRegisteredTaskEventHandler();
    
    [Signal]
    public delegate void IncreaseCompletedTaskEventHandler();

    public override void _Ready()
    {
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
        _hSlider.Value = TaskCompleted;
    }

}