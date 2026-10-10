using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Serializers;
using RubyMarshalCS.Serialization;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.Structures;

/// <summary>
/// Up to three dimensional array of ints, the same for MV and VXAce.
/// Cells are stored x first, then y, then z: <c>index = x + y * SizeX + z * SizeX * SizeY</c>.
/// <c>.rvdata2</c> stores cells as 16-bit values, saving a cell outside of that range throws.
/// </summary>
[RubyObject("Table")]
[RubyUserSerializer(typeof(TableSerializer), "")]
public class Table : RubySerializationEntity
{
    [RubyProperty("@num_of_dimensions")]
    public int NumOfDimensions;

    [RubyProperty("@xsize")]
    public int SizeX;

    [RubyProperty("@ysize")]
    public int SizeY;

    [RubyProperty("@zsize")]
    public int SizeZ;

    [RubyProperty("@num_of_elements")]
    public int NumOfElements;

    [RubyProperty("@elements")]
    public List<int> Elements = new();

    public Table()
    {
    }

    /// <summary>Table filled with zeros.</summary>
    public Table(int sizeX, int sizeY = 1, int sizeZ = 1)
    {
        Elements = new List<int>(new int[sizeX * sizeY * sizeZ]);
        Reshape(sizeX, sizeY, sizeZ);
    }

    /// <summary>One-dimensional table over the given cells.</summary>
    public static Table FromCells(List<int> cells)
    {
        var table = new Table { Elements = cells };
        table.Reshape(cells.Count);

        return table;
    }

    public int Count => Elements.Count;

    /// <summary>Changes the dimensions without touching the cells.</summary>
    public void Reshape(int sizeX, int sizeY = 1, int sizeZ = 1)
    {
        SizeX = sizeX;
        SizeY = sizeY;
        SizeZ = sizeZ;
        NumOfDimensions = sizeZ > 1 ? 3 : sizeY > 1 ? 2 : 1;
        NumOfElements = Elements.Count;
    }

    public int this[int x]
    {
        get => this[x, 0, 0];
        set => this[x, 0, 0] = value;
    }

    public int this[int x, int y]
    {
        get => this[x, y, 0];
        set => this[x, y, 0] = value;
    }

    public int this[int x, int y, int z]
    {
        get => Elements[Index(x, y, z)];
        set => Elements[Index(x, y, z)] = value;
    }

    /// <summary>Reads a cell; false when the position is outside the dimensions or the cells are shorter than declared.</summary>
    public bool TryGet(int x, int y, int z, out int value)
    {
        value = 0;
        if (!InRange(x, y, z))
            return false;

        var index = x + y * SizeX + z * SizeX * SizeY;
        if (index >= Elements.Count)
            return false;

        value = Elements[index];
        return true;
    }

    private bool InRange(int x, int y, int z) =>
        (uint)x < (uint)SizeX && (uint)y < (uint)SizeY && (uint)z < (uint)SizeZ;

    private int Index(int x, int y, int z)
    {
        if (!InRange(x, y, z))
            throw new IndexOutOfRangeException($"[{x}, {y}, {z}] is outside of {SizeX}x{SizeY}x{SizeZ}");

        return x + y * SizeX + z * SizeX * SizeY;
    }
}
