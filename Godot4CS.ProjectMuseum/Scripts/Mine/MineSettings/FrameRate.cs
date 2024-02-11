using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MineSettings;

public partial class FrameRate : Node
{
	public override void _Ready()
	{
		Engine.MaxFps = 120;
	}
}