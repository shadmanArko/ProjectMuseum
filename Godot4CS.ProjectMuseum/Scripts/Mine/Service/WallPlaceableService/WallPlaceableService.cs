using System.Collections.Generic;
using System.Linq;
using ProjectMuseum.Models;

namespace Godot4CS.ProjectMuseum.Scripts.Mine.Service.WallPlaceableService;

public class WallPlaceableService
{
    public WallPlaceable GetConsumableByVariant(string variant, List<WallPlaceable> wallPlaceables)
    {
        var wallPlaceable = wallPlaceables.FirstOrDefault(temp => temp.Variant == variant);
        return wallPlaceable;
    }
}