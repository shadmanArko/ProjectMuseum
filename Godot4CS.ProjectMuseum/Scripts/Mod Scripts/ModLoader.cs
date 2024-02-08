using Godot;
using System;
using System.Reflection;

public partial class ModLoader : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Assembly.LoadFile("E:\\Godot Projects\\Godot Mod Testing\\.godot\\mono\\temp\\bin\\Debug\\Godot Mod Testing.dll");
		var success = ProjectSettings.LoadResourcePack("C:\\Users\\USER\\Desktop\\DLC01.zip");
		if (success)
		{
			GD.Print("Success");
			var importedScene = (PackedScene)ResourceLoader.Load("res://mod_scene.tscn");
			GetTree().ChangeSceneToPacked(importedScene);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
