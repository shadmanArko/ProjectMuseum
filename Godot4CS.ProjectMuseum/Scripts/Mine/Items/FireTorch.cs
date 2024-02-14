using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Items;

public partial class FireTorch : Node
{
	[Export] private AnimationPlayer _animationPlayer;
	public override void _Ready()
	{
		PlayTorchAnimation();
	}

	public void PlayTorchAnimation()
	{
		_animationPlayer.Play("fireTorch");
	}
}