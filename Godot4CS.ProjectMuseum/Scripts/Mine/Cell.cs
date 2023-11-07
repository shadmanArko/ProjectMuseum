
using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.MineScripts;

public class Cell
{
	public Vector2 Pos;
	public bool IsBreakable;
	public bool HasArtifact;
	public bool IsInstantiated;
	public int BreakStrength;
	

	public Cell(bool isBreakable, bool hasArtifact, int breakStrength)
	{
		IsBreakable = isBreakable;
		HasArtifact = hasArtifact;
		BreakStrength = breakStrength;
	}
}
