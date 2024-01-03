using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MineTimeSystem;

public partial class MineTimePanel : PanelContainer
{
	[Export] private Label _time;

	public override void _EnterTree()
	{
		SubscribeToActions();
	}

	public override void _Ready()
	{
		
	}

	private void SubscribeToActions()
	{
		MineActions.OnTimeUpdated += UpdateMineTime;
	}

	private void UpdateMineTime(int minutes, int hours, int days, int months, int years)
	{
		_time.Text = $"Day(s) {days:D2}, {hours:D2}:{minutes:D2}";
	}
}