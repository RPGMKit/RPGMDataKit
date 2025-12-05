namespace RPGMakerLibCoreV2.Data.Enums
{

    /**
     32*8*

    tilesorder:
    +B 0 - 255 (1 per square)
    +C 256 - 511 (1 per square)
    +D 512 - 767 (1 per square)
    +E 768 - 1023 (1 per square)
    ?????
    +A5 1536 - 1663 (128) (1 per square)
    ?????
    + A1 2048 - 2815 (768) (48 variations per square in tile editor)
    + A2 2816 - 4351 (1536) (48 variations per square in tile editor)
    + A3 4352 - 5887 (1536) (48 variations per square in tile editor)
    + A4 5888 - 8191 (2304) (48 variations per square in tile editor)
     */
    public enum TileFlags : int
    {
        None = 0x0,
        PassNoDown = 0x1,
        PassNoLeft = 0x2,
        PassNoRight = 0x4,
        PassNoUp = 0x8,
        Impassable = PassNoUp | PassNoRight | PassNoDown | PassNoLeft,
        IsHigherTile = 0x10, // [*] no effect on passage, can pass under
        Ladder = 0x20,
        Bush = 0x40,
        Counter = 0x80,
        DamageFloor = 0x100,
        BoatPassable = 0x200,
        ShipPassable = 0x400,
        AirhsipLandOk = 0x800,
        TerrainTag1 = 0x1000,
        TerrainTag2 = 0x2000,
        TerrainTag3 = 0x3000,
        TerrainTag4 = 0x4000,
        TerrainTag5 = 0x5000,
        TerrainTag6 = 0x6000,
        TerrainTag7 = 0x7000,
    }
}