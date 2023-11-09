using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public class MineGenerationVariables
{
    public Cell[,] Cells;
    public int CellSize = 20;
    public int GridWidth = 49;
    public int GridLength = 64;

    public MineGenerationView MineGenView;
}