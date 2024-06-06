using Godot;
using System;
using System.Threading.Tasks;
using Godot4CS.ProjectMuseum.Scripts.Museum.Museum_Actions;

public partial class GateOpenCloseSign : TextureRect
{
	[Export] private Texture2D _texture;
	[Export] private int _numberOfFrames = 7;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MuseumActions.OnClickMuseumGateToggle += OnClickMuseumGateToggle;
	}

	private  void OnClickMuseumGateToggle(bool gateOpen)
	{
		if (gateOpen)
		{
			OpenGate();
		}else CloseGate();
		GD.Print($"Gate {gateOpen}");
	}

	private async void OpenGate()
	{
		
		for (int i = 1; i < _numberOfFrames; i++)
		{
			
			AtlasTexture atlasTexture = new AtlasTexture();
			atlasTexture.Atlas = _texture;
			atlasTexture.Region = new Rect2(56*i, 40, 56, 40);
			Texture = atlasTexture;
			GD.Print($" open Gate  {atlasTexture.Region}");
			await Task.Delay(100);
		}
	}
	private async void CloseGate()
	{
		
		for (int i = 1; i < _numberOfFrames; i++)
		{
			AtlasTexture atlasTexture = new AtlasTexture();
			atlasTexture.Atlas = _texture;
			atlasTexture.Region = new Rect2(56*i, 0, 56, 40);
			Texture = atlasTexture;
			GD.Print($" close Gate  {atlasTexture.Region}");
			await Task.Delay(100);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		MuseumActions.OnClickMuseumGateToggle -= OnClickMuseumGateToggle;

	}
}
