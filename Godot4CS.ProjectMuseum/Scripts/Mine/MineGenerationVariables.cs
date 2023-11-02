using Godot4CS.ProjectMuseum.Scripts.MineScripts;

namespace Godot4CS.ProjectMuseum.Scripts.Mine;

public class MineGenerationVariables
{
    public Cell[,] Grid;
    public int CellSize = 16;
    public int GridWidth = 35;
    public int GridLength = 64;

    public MineGenerationView MineGenView;
}