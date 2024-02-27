using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects.Variants;

public partial class FireTorch : Node2D
{
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private Sprite2D _torchSprite;

	public override void _Ready()
	{
		PlayAnimation();
	}

	private void PlayAnimation()
	{
		_animationPlayer.Play("fireTorch");
	}

	
}