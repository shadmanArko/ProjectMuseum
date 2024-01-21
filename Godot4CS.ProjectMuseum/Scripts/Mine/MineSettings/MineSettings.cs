using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MineSettings;

public partial class MineSettings : Node
{
	public override void _Ready()
	{
		Engine.MaxFps = 100;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		GD.Print($"FPS: {Engine.GetFramesPerSecond()}");
	}
}