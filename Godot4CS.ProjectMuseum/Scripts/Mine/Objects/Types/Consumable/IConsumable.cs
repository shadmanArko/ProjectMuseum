namespace Godot4CS.ProjectMuseum.Scripts.Mine.Objects.Types.Consumable;

public interface IConsumable
{
    bool CheckEligibility();
    void ApplyStatEffect();
}