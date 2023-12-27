using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Museum.Tutorial_System;

public partial class MineTutorial : Node
{
	public override void _Ready()
	{
		Museum_Actions.MuseumActions.PlayTutorial?.Invoke(6);
	}

	
}