using Godot;
using System;

public partial class StaticBody2D : Godot.StaticBody2D
{
	private Tooltip4 _tooltip4;

	public string text4Tooltip;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_tooltip4 = GetNode<Tooltip4>("../../Tooltip4");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void OnMouseEntered()
	{
		_tooltip4.Show();
	}

	private void OnMouseExited()
	{
		_tooltip4.Hide();
	}
}
