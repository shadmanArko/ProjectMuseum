

namespace ProjectMuseum.Models;

public class Cell
{
	public string Id { get; set; }
	public float PositionX{ get; set; }
	public float PositionY{ get; set; }
	public bool IsBreakable{ get; set; }
	public bool IsRevealed { get; set; }
	public bool HasArtifact{ get; set; }
	public bool IsInstantiated{ get; set; }
	public int BreakStrength{ get; set; }
}
