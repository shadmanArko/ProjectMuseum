using Godot;

namespace Godot4CS.ProjectMuseum.Scripts.Museum;

public partial class MuseumTileEditor: Node2D
{
    [Export] private TileMap _tileMap;
    public override void _Ready()
    {
        base._Ready();
        _tileMap.SetCell(0, new Vector2I( 0, 0), 0, Vector2I.Zero);
    }
    
}