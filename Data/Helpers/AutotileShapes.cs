namespace RPGMDataKit.Data.Helpers;

/// <summary>
/// The shape of an autotile from what is round it: the number (0-47) that goes into the id (<see cref="TileIds.MakeAutotile"/>) and tells the engine which quarters of the picture to draw.
/// "Connected" means that the neighbour is of the same kind (or one of the other things that the editors join, see <see cref="MapAutotileExtensions"/>). The numbers are the editors' own: they are the
/// order of <c>FLOOR_AUTOTILE_TABLE</c>, <c>WALL_AUTOTILE_TABLE</c> and <c>WATERFALL_AUTOTILE_TABLE</c> of the engine (see <c>.claude-docs/maps-and-tiles.md</c>).
/// </summary>
public static class AutotileShapes
{
    /// <summary>
    /// The shape of an autotile of 48 shapes (water, ground, the top of a wall) from its 8 neighbours: 0 has all of them, 46 none of the four sides. A corner counts only when both sides of it are
    /// connected (the other corners are not seen in the picture). 47 is never given: the editors draw a tile with nothing round it as 46.
    /// </summary>
    public static int Floor(bool up, bool down, bool left, bool right, bool upLeft, bool upRight, bool downLeft, bool downRight)
    {
        // the notch of a corner: both sides of it are connected, the corner is not
        var topLeft = up && left && !upLeft;
        var topRight = up && right && !upRight;
        var bottomLeft = down && left && !downLeft;
        var bottomRight = down && right && !downRight;
        var apart = (up ? 0 : 1) + (down ? 0 : 1) + (left ? 0 : 1) + (right ? 0 : 1);

        switch (apart)
        {
            case 0:
                return (topLeft ? 1 : 0) + (topRight ? 2 : 0) + (bottomRight ? 4 : 0) + (bottomLeft ? 8 : 0);
            case 1:
                if (!left)
                    return 16 + (topRight ? 1 : 0) + (bottomRight ? 2 : 0);
                if (!up)
                    return 20 + (bottomRight ? 1 : 0) + (bottomLeft ? 2 : 0);
                if (!right)
                    return 24 + (bottomLeft ? 1 : 0) + (topLeft ? 2 : 0);

                return 28 + (topLeft ? 1 : 0) + (topRight ? 2 : 0);
            case 2:
                if (!left && !right)
                    return 32;
                if (!up && !down)
                    return 33;
                if (!up && !left)
                    return 34 + (bottomRight ? 1 : 0);
                if (!up && !right)
                    return 36 + (bottomLeft ? 1 : 0);
                if (!down && !right)
                    return 38 + (topLeft ? 1 : 0);

                return 40 + (topRight ? 1 : 0);
            case 3:
                if (down)
                    return 42;
                if (right)
                    return 43;
                if (up)
                    return 44;

                return 45;
            default:
                return 46;
        }
    }

    /// <summary>The shape of an autotile of 16 shapes (a roof, the side of a wall) from its 4 sides: 0 has all of them, 15 none; a side that is not connected adds 1 (left), 2 (up), 4 (right), 8 (down).</summary>
    public static int Wall(bool up, bool down, bool left, bool right) => (left ? 0 : 1) + (up ? 0 : 2) + (right ? 0 : 4) + (down ? 0 : 8);

    /// <summary>The shape of a waterfall (4 shapes) from its sides: 0 has both, a side that is not connected adds 1 (left), 2 (right).</summary>
    public static int Waterfall(bool left, bool right) => (left ? 0 : 1) + (right ? 0 : 2);
}
