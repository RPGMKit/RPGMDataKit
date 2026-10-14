namespace RPGMDataKit.Data.Structures.Interfaces;

public interface ISkill : IUsableItem
{
    public int STypeId { get; set; }

    public int MpCost { get; set; }

    public int TpCost { get; set; }

    public string Message1 { get; set; }

    public string Message2 { get; set; }

    public int RequiredWTypeId1 { get; set; }

    public int RequiredWTypeId2 { get; set; }
}
