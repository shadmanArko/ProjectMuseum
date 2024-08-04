using System.IO;
using System.Linq;
using Godot;
using Godot4CS.ProjectMuseum.Scripts.Dependency_Injection;
using Godot4CS.ProjectMuseum.Scripts.Mine.PlayerScripts;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;
using ProjectMuseum.DTOs;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.MiniGames;

public partial class DiscoveredArtifactVisualizer : Node2D
{
	private PlayerControllerVariables _playerControllerVariables;
	private RawArtifactDTO _rawArtifactDto;
	
	[Export] private CanvasLayer _canvasLayer;
	[Export] private Label _artifactName;
    [Export] private Sprite2D _artifactSprite;
    [Export] private Label _artifactDescription;
	[Export] private Button _okayButton;

	#region Icon Sprites

	[Export] private Sprite2D _eraIcon;
	[Export] private Sprite2D _regionIcon;
	[Export] private Sprite2D _objectClassIcon;
	[Export] private Sprite2D _objectSizeIcon;
	[Export] private Sprite2D _Material1Icon;
	[Export] private Sprite2D _Material2Icon;

	#endregion

	#region Folder Paths

	[Export] private string _eraFolderPath;
	[Export] private string _regionFolderPath;
	[Export] private string _objectClassFolderPath;
	[Export] private string _objectSizeFolderPath;
	[Export] private string _materialFolderPath;

	#endregion

	public override void _Ready()
	{
		InitializeDiInstaller();
		SubscribeToActions();

		_canvasLayer.Visible = false;
	}
    
	private void InitializeDiInstaller()
	{
		_rawArtifactDto = ServiceRegistry.Resolve<RawArtifactDTO>();
		_playerControllerVariables = ServiceRegistry.Resolve<PlayerControllerVariables>();
	}
	
	private void SubscribeToActions()
	{
		MineActions.OnArtifactSuccessfullyRetrieved += ShowDiscoveredArtifactVisualizerUi;
		_okayButton.Pressed += HideDiscoveredArtifactVisualizerUi;
	}

	private void ShowDiscoveredArtifactVisualizerUi(Artifact artifact)
	{
		_playerControllerVariables.CanMove = false;
		var rawArtifactFunctional =
			_rawArtifactDto.RawArtifactFunctionals.FirstOrDefault(tempArtifact => tempArtifact.Id == artifact.RawArtifactId);
		var rawArtifactDescriptive =
			_rawArtifactDto.RawArtifactDescriptives.FirstOrDefault(tempArtifact => tempArtifact.Id == artifact.RawArtifactId);
		
		GD.Print($"{artifact.RawArtifactId},");
		
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

		GD.Print("SHOWING ARTIFACT FROM ARTIFACT ID IN DISCOVERY ARTIFACT VISUALIZER");
		_canvasLayer.Visible = true;
		_artifactName.Text = rawArtifactDescriptive.ArtifactName;
		_artifactDescription.Text = $"{rawArtifactDescriptive.Description}";

		_eraIcon.Texture = GetTexture(_eraFolderPath, rawArtifactFunctional.Era);
		GD.Print($"Era is {rawArtifactFunctional.Era}");
			
		// 	$"[p][font_size={11}]{rawArtifactFunctional.Era}, {rawArtifactFunctional.Region},[/font_size][/p]" +
		// 	$"[p][font_size={11}]{rawArtifactFunctional.Object}, {rawArtifactFunctional.ObjectClass}," +
		// 	$" {rawArtifactFunctional.ObjectSize}[/font_size][/p]";
		// var materialText = "[font_size={11}][p]";
		// foreach (var material in rawArtifactFunctional.Materials)
		// {
		// 	materialText += $"{material}, ";
		// }

		// descriptionText += materialText+"[/p][/font_size]";
		// _artifactDescription.Text = descriptionText;
		_artifactSprite.Texture = GD.Load<Texture2D>(rawArtifactFunctional.LargeImageLocation);
	}

	private Texture2D GetTexture(string folderPath, string iconFileName)
	{
		using var dir = DirAccess.Open(folderPath);
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			
			while (fileName != "")
			{
				if (!dir.CurrentIsDir() && fileName == iconFileName+".png")
				{
					var texturePath = Path.Combine(folderPath, fileName);
					var texture = GD.Load<Texture2D>(texturePath);

					if (texture == null)
					{
						GD.PrintErr($"FATAL ERROR: Could not find texture");
						continue;
					}
					GD.Print("Texture: " + texturePath);
					return texture;
				}
				fileName = dir.GetNext();
			}
		}

		return null;
	}

	private void OnOkayButtonPressed()
	{
		MuseumActions.OnPlayerPerformedTutorialRequiringAction?.Invoke("MiniGamesWon");
		HideDiscoveredArtifactVisualizerUi();
		MineActions.OnMiniGameEnded?.Invoke();
	}

	private void HideDiscoveredArtifactVisualizerUi()
	{
		_canvasLayer.Visible = false;
	}

	#region Finalizers

	private void UnsubscribeToActions()
	{
		MineActions.OnArtifactSuccessfullyRetrieved -= ShowDiscoveredArtifactVisualizerUi;
	}

	public override void _ExitTree()
	{
		UnsubscribeToActions();
	}

	#endregion
}