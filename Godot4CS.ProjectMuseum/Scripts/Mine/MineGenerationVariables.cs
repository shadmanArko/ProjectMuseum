using Godot4CS.ProjectMuseum.Scripts.MineScripts;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public class MineGenerationVariables
{
    public Cell[,] Cells;
    public int CellSize = 16;
    public int GridWidth = 35;
    public int GridLength = 64;

    public MineGenerationView MineGenView;
}