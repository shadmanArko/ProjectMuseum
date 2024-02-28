using Godot;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Tests.DragAndDrop;

public static class GameManager
{
    public static TileMap tileMap;
    public static AStarNode[,] outSideMuseumNodes;
    public static bool isMuseumGateOpen;
}