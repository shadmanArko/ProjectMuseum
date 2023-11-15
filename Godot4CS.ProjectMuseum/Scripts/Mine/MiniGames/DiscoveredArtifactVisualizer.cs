using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;

public partial class DiscoveredArtifactVisualizer : CanvasLayer
{
	[Export] private Sprite2D _artifactSprite;
	[Export] private Button _okayButton;

	public override void _Ready()
	{
		
	}

	public void OnOkayButtonPressed()
	{
		Visible = false;
		MineActions.OnArtifactDiscoveryOkayButtonPressed?.Invoke();
		_ExitTree();
	}
	
}