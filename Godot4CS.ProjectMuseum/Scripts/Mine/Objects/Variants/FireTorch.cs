using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects.Variants;

public partial class FireTorch : Types.WallPlaceable.WallPlaceableObject
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

	override 
	public void SetSpriteColorToGreen()
	{
		_torchSprite.Modulate = Colors.Green;
	}
    
	override 
	public void SetSpriteColorToRed()
	{
		_torchSprite.Modulate = Colors.Red;
	}

	override 
	public void SetSpriteColorToDefault()
	{
		_torchSprite.Modulate = Colors.White;
	}
}