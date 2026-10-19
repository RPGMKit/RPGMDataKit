namespace RPGMDataKit.Data.Enums
{
    /// <summary>
    /// The shadows of a square of a map: one bit for each quarter of the tile, the same in VX Ace and MV (<c>Tilemap._drawShadow</c>: the bit <c>i</c> is the quarter
    /// at column <c>i % 2</c>, row <c>i / 2</c>). See <see cref="Helpers.MapTileExtensions"/>.
    /// </summary>
    [Flags]
    public enum ShadowFlag: int
    {
        None = 0,
        TopLeft = 1,
        TopRight = 2,
        BottomLeft = 4,
        BottomRight = 8,
        All = TopLeft | TopRight | BottomLeft | BottomRight,
    }
}
