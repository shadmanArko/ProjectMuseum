using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectMuseum.Models;

public partial class DraggableNewPreview : ColorRect
{
	[Export] private Label _nameOfDraggable;
	[Export] private TextureRect _artifactIcon;
	[Export] private Control _artifactTagParent;
	[Export] private PackedScene _artifactTag;
	public Artifact Artifact;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void Initialize(Artifact artifact, List<RawArtifactDescriptive> rawArtifactDescriptives, List<RawArtifactFunctional> rawArtifactFunctionals)
	{
		if (artifact == null)
		{
			//GD.Print("No artifact");
			return;
		}

		RawArtifactDescriptive rawArtifactDescriptive =
			rawArtifactDescriptives.FirstOrDefault(descriptive => descriptive.Id == artifact.RawArtifactId);
		RawArtifactFunctional rawArtifactFunctional =
			rawArtifactFunctionals.FirstOrDefault(descriptive => descriptive.Id == artifact.RawArtifactId);
		_nameOfDraggable.Text = rawArtifactDescriptive.ArtifactName;
		_artifactIcon.Texture = GD.Load<Texture2D>(rawArtifactFunctional.LargeImageLocation);
		
		InstantiateArtifactTag(rawArtifactFunctional.Era);
		InstantiateArtifactTag(rawArtifactFunctional.Region);
		InstantiateArtifactTag(rawArtifactFunctional.Object);
		foreach (var material in rawArtifactFunctional.Materials)
		{
			InstantiateArtifactTag(material);
		}
		Artifact = artifact;
	}

	private void InstantiateArtifactTag(string tag)
	{
		var instance = _artifactTag.Instantiate();
		_artifactTagParent.AddChild(instance);
		instance.GetNode<ExhibitEditorArtifactTag>(".").Initialize(tag);
	}
}
