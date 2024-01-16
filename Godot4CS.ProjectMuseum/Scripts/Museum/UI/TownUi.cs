using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class TownUi : Panel
{
	[Export] private Control _popUpUi;
	[Export] private Sprite2D _indicatorForHouseOfBuddy;
	private Timer _timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = new Timer();
		AddChild(_timer);
		_timer.Timeout += TimerOnTimeout;
		MuseumActions.OnPlayerClickedAnEmptyHouse += OnPlayerClickedAnEmptyHouse;
		MuseumActions.OnPlayerPerformedTutorialRequiringAction += OnPlayerPerformedTutorialRequiringAction;

	}

	private void OnPlayerPerformedTutorialRequiringAction(string obj)
	{
		if (obj == "FoundDiggingBuddy")
		{
			_indicatorForHouseOfBuddy.Visible = false;

		}
	}

	private void TimerOnTimeout()
	{
		_popUpUi.Visible = false;
	}

	private  void OnPlayerClickedAnEmptyHouse()
	{
		_timer.Start(1);
		_popUpUi.Visible = true;
		_indicatorForHouseOfBuddy.Visible = true;

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_timer.Timeout -= TimerOnTimeout;
		MuseumActions.OnPlayerClickedAnEmptyHouse -= OnPlayerClickedAnEmptyHouse;
		MuseumActions.OnPlayerPerformedTutorialRequiringAction -= OnPlayerPerformedTutorialRequiringAction;

	}
}
