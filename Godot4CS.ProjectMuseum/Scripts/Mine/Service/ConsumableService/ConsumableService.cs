using System.Collections.Generic;
using System.Linq;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Service.ConsumableService;

public class ConsumableService
{
    public Consumable GetConsumableByVariant(string variant, List<Consumable> consumables)
    {
        var consumable = consumables.FirstOrDefault(temp => temp.Variant == variant);
        return consumable;
    }
}