using Godot;
using System;

public partial class ExhibitEditorArtifactTag : ColorRect
{
	[Export] public Label artifactTag;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void Initialize(string tag)
	{
		artifactTag.Text = tag;
		Size = artifactTag.Size + new Vector2(2, 0);
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
