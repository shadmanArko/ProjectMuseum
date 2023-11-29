namespace ProjectMuseum.Models;

public class RawMob
{
    public string Id { get; set; }
    public string Class { get; set; }
    public int Health { get; set; }
    public int MovementSpeed { get; set; }
    public int DamagePerAttack { get; set; }
    public int AggroRange { get; set; }
    public bool StatusEffectOnContact { get; set; }
    public string StatusEffect { get; set; }
    public bool CanFly { get; set; }
    public bool CanDig { get; set; }
    public string Region { get; set; }
}