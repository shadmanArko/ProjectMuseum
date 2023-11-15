using System.Threading.Tasks;
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;

public partial class DiscoveredArtifactVisualizer : CanvasLayer
{
	[Export] private Sprite2D _artifactSprite;
	
	public async Task ShowArtifact()
	{
		while (_artifactSprite.Scale.X <= 0.5f)
		{
			_artifactSprite.Scale += new Vector2(0.016f, 0.016f);
			await Task.Delay(100);
		}

		await Task.Yield();
		Visible = false;
	}
}