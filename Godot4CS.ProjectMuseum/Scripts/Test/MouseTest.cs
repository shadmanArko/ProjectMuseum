using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Test;

public partial class MouseTest : Node
{
	public override void _Ready()
	{
		SetProcess(false);
	}

	public override void _Process(double delta)
	{
		GD.Print("left mouse pressed");
	}

	private bool _isMouseHeld;
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_left_click"))
		{
			GD.Print("left mouse clicked");
			SetProcess(true);
		}

		if (@event.IsActionReleased("ui_left_click"))
		{
			GD.Print("left mouse released");
			SetProcess(false);
		}
	}
}