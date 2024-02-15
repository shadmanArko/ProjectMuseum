using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Items;

public partial class FireTorch : Node2D
{
	[Export] private AnimationPlayer _animationPlayer;
	[Export] private Sprite2D _torchSprite;

	public override void _Ready()
	{
		PlayTorchAnimation();
	}

	public void PlayTorchAnimation()
	{
		_animationPlayer.Play("fireTorch");
	}

	public void SetSpriteColorToGreen()
	{
		_torchSprite.Modulate = Colors.Green;
	}
    
	public void SetSpriteColorToRed()
	{
		_torchSprite.Modulate = Colors.Red;
	}

	public void SetSpriteColorToDefault()
	{
		_torchSprite.Modulate = Colors.White;
	}
}