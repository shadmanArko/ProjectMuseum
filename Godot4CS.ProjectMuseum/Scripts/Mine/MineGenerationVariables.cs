using System.Linq;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public class MineGenerationVariables
{
    public global::ProjectMuseum.Models.Mine Mine;
    public MineGenerationView MineGenView;
    public int BrokenCells { get; set; }

    public Cell GetCell(Vector2I tilePos)
    {
        return Mine.Cells.FirstOrDefault(cell1 => cell1.PositionX == tilePos.X && cell1.PositionY == tilePos.Y);
    }
}