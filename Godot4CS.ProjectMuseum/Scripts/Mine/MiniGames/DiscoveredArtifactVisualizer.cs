using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using ProjectMuseum.DTOs;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;

public partial class DiscoveredArtifactVisualizer : Node2D
{
	private RawArtifactDTO _rawArtifactDto;
	
	[Export] private CanvasLayer _canvasLayer;
	[Export] private Label _artifactName;
    [Export] private Sprite2D _artifactSprite;
	[Export] private Button _okayButton;

	public override void _Ready()
	{
		InitializeDiInstaller();
		SubscribeToActions();

		_canvasLayer.Visible = false;
	}
    
	private void InitializeDiInstaller()
	{
		_rawArtifactDto = ServiceRegistry.Resolve<RawArtifactDTO>();
	}
	
	private void SubscribeToActions()
	{
		_okayButton.Pressed += HideDiscoveredArtifactVisualizerUi;
	}

	public void ShowDiscoveredArtifactVisualizerUi(string artifactId)
	{
		var rawArtifactFunctional =
			_rawArtifactDto.RawArtifactFunctionals.FirstOrDefault(tempArtifact => tempArtifact.Id == artifactId);
		var rawArtifactDescriptive =
			_rawArtifactDto.RawArtifactDescriptives.FirstOrDefault(tempArtifact => tempArtifact.Id == artifactId);
		
		if (rawArtifactFunctional == null)
		{
			GD.Print("ARTIFACT FUNCTIONAL IS NULL");
			return;
		}
		
		if (rawArtifactDescriptive == null)
		{
			GD.Print("ARTIFACT DESCRIPTIVE IS NULL");
			return;
		}

		_canvasLayer.Visible = true;
		_artifactName.Text = rawArtifactDescriptive.ArtifactName;
		_artifactSprite.Texture = GD.Load<Texture2D>(rawArtifactFunctional.LargeImageLocation);
	}

	private void HideDiscoveredArtifactVisualizerUi()
	{
		Visible = false;
		_ExitTree();
	}
	
}