using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public partial class MineGenerationView : TileMap
{
	[Export] public TileMap TileMap;
	[Export] public int TileSourceId;
	[Export] public int WoodArtifactSourceId;
	[Export] public int SilverArtifactSourceId;
	[Export] public int GoldArtifactSourceId;
	[Export] public int NewNewTilesSourceId;
	[Export] public int TileCrackSourceId;
}