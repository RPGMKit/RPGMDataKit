namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IWeapon : IEquipItem
{
    public int WTypeId { get; set; }

    public int AnimationId { get; set; }
}
