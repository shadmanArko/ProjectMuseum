using Godot;
using System;

public partial class ColorRect : Godot.ColorRect
{
	private MineSceneTooltip _mineSceneTooltip;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_mineSceneTooltip = GetNode<MineSceneTooltip>("../../MineSceneTooltip");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void OnMouseEntered()
	{
		_mineSceneTooltip.Show();
		
	}
	
	private void OnMouseExited()
	{
		_mineSceneTooltip.Hide();
	}
}
