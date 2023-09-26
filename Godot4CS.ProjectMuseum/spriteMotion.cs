using Godot;
using System;

public partial class spriteMotion : Sprite2D
{
	private float topLimit;
	private float bottomLimit;
	public override void _Ready()
	{
		topLimit = GetTransform().Origin.Y - 5;
		bottomLimit = GetTransform().Origin.Y + 5;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// 	var currentTransform = GetTransform().Origin;
	// 	//GetTransform().Origin.MoveToward(currentTransform.X, )
	// }
}
