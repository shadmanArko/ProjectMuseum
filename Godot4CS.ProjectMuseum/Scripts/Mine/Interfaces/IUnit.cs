using Godot;
using Godot4CS.ProjectMuseum.Scripts.Mine.Enums;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Interfaces;

public interface IUnit
{
    public string Id { get; set; }
    public NavigationAgent2D NavAgent { get; set; }
    public Timer TrackTimer { get; set; }
    public EnemyState State { get; set; }
    public bool IsAffectedByGravity { get; set; }
}