
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts;

public partial class Cell : Node2D
{
	public Vector2 Pos;
	public bool IsBreakable;
	public bool HasArtifact;
	public int BreakStrength;
	public Sprite2D Sprite2D;

	public Cell(Vector2 position, bool isBreakable, bool hasArtifact, int breakStrength)
	{
		Pos = position;
		IsBreakable = isBreakable;
		HasArtifact = hasArtifact;
		BreakStrength = breakStrength;
	}
}
