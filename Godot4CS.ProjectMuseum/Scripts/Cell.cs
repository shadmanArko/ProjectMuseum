using Godot;

namespace Godot4CS.ProjectMuseum.Scripts;

public class Cell : Node2D
{
    public bool IsBreakable = true;
    public bool HasArtifact = false;
    public int BreakStrength = 0;
}