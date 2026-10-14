namespace RPGMDataKit.Data.Structures.Interfaces;

public interface IActor : IBaseItem, IHasFeatures
{
    public int ClassId { get; set; }

    public string NickName { get; set; }

    public string CharacterName { get; set; }

    public int CharacterIndex { get; set; }

    public string FaceName { get; set; }

    public int FaceIndex { get; set; }

    /// <summary>The picture of the side view battle; exists in MV only, VX Ace throws <c>DoesNotExistInVersionException</c>.</summary>
    public string BattlerName { get; set; }
}
