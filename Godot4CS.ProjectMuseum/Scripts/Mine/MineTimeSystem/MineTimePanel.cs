using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MineTimeSystem;

public partial class MineTimePanel : PanelContainer
{
	[Export] private int _minutes;
	[Export] private int _hours;
	public override void _Ready()
	{
		SubscribeToActions();
	}

	private void SubscribeToActions()
	{
		MineActions.OnEachMinutePassed += MinutesPassed;
		MineActions.OnEachHourPassed += HoursPassed;
	}

	private void HoursPassed(int hours)
	{
		
	}

	private void MinutesPassed(int minutes)
	{
		
	}
}