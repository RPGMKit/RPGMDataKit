using RPGMDataKit.Data.Enums;
using RPGMDataKit.Storages;

namespace RPGMDataKit.Data.Structures.Attributes;

/// <summary>
/// A constant that the editor gives to a parameter of a new command (see <see cref="EditorDefaultBaseAttribute"/>): a number for a number or an
/// enum, a bool, a text. For example <c>[EditorDefault(60, StorageVersion.VXAce)]</c> on the frames of a Wait.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class EditorDefaultAttribute(object value, params StorageVersion[] versions) : EditorDefaultBaseAttribute(versions)
{
    public object Value { get; } = value;

    public override object? CreateValue(TypeMap map, Type propertyType)
    {
        var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        return type.IsEnum ? Enum.ToObject(type, Value) : type == typeof(object) ? Value : Convert.ChangeType(Value, type);
    }
}
