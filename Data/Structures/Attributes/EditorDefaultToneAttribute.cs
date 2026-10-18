using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Storages;

namespace RPGMDataKit.Data.Structures.Attributes;

/// <summary>A tone that the editor gives to a parameter of a new command (see <see cref="EditorDefaultBaseAttribute"/>), for a property of <see cref="ITone"/>.</summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class EditorDefaultToneAttribute(int red, int green, int blue, int gray, params StorageVersion[] versions) : EditorDefaultBaseAttribute(versions)
{
    public int Red { get; } = red;

    public int Green { get; } = green;

    public int Blue { get; } = blue;

    public int Gray { get; } = gray;

    public override object? CreateValue(TypeMap map, Type propertyType)
    {
        var tone = map.New<ITone>();
        tone.Red = Red;
        tone.Green = Green;
        tone.Blue = Blue;
        tone.Gray = Gray;

        return tone;
    }
}
