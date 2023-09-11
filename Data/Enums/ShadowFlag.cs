namespace RPGMakerLibCoreV2.Data.Enums
{
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
