using Godot;
using System;
using Godot4CS.ProjectMuseum.Scripts.Museum.Managers;
using Godot4CS.ProjectMuseum.Service.MuseumServices;

public partial class MainMenuReferanceManager : Node
{
	public static MainMenuReferanceManager Instance;
	[Export] public PlayerInfoServices PlayerInfoServices;
	public override void _Ready()
	{
		base._Ready();
		Instance ??= this;
	}
}
