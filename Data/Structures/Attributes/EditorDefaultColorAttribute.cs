using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Storages;

namespace RPGMDataKit.Data.Structures.Attributes;

/// <summary>A color that the editor gives to a parameter of a new command (see <see cref="EditorDefaultBaseAttribute"/>), for a property of <see cref="IColor"/>.</summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class EditorDefaultColorAttribute(int red, int green, int blue, int alpha, params StorageVersion[] versions) : EditorDefaultBaseAttribute(versions)
{
    public int Red { get; } = red;

    public int Green { get; } = green;

    public int Blue { get; } = blue;

    public int Alpha { get; } = alpha;

    public override object? CreateValue(TypeMap map, Type propertyType)
    {
        var color = map.New<IColor>();
        color.Red = Red;
        color.Green = Green;
        color.Blue = Blue;
        color.Alpha = Alpha;

        return color;
    }
}
