using ProjectMuseum.Models.MIne.Equipables;

namespace ProjectMuseum.DTOs;

public class EquipableDTO
{
    public List<EquipableMelee> MeleeEquipables { get; set; }
    public List<EquipableRange> RangedEquipables { get; set; }
    public List<EquipablePickaxe> PickaxeEquipables { get; set; }
}