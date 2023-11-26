using System.Linq;
using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public class MineGenerationVariables
{
    public global::ProjectMuseum.Models.Mine Mine;
    //public Cell[,] Cells;
    public int CellSize = 20;
    public int GridWidth = 49;
    public int GridLength = 64;

    public MineGenerationView MineGenView;

    public Cell GetCell(Vector2I tilePos)
    {
        return Mine.Cells.FirstOrDefault(cell1 => cell1.PositionX == tilePos.X && cell1.PositionY == tilePos.Y);
    }
}