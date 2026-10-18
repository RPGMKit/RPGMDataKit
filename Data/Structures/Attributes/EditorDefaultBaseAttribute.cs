using RPGMDataKit.Data.Enums;
using RPGMDataKit.Storages;

namespace RPGMDataKit.Data.Structures.Attributes;

/// <summary>
/// The value that the editor gives to a parameter of a new command, for a property of a command: a command that is made in code and put into a
/// list gets it where the parameter was not set (a parameter that is set to 0 or to false stays). The attribute makes the value itself
/// (<see cref="CreateValue"/>), so a value can be a number, a text, a tone, a list: <see cref="EditorDefaultAttribute"/> for a constant,
/// <see cref="EditorDefaultToneAttribute"/>, <see cref="EditorDefaultColorAttribute"/>, <see cref="EditorDefaultListAttribute"/>.
/// <para>
/// Without versions the value is the same in all makers; with versions, only in those (it was looked at in the editor of that maker, the others
/// are not known). Several attributes can be on a property: the one for the maker of the list is taken, else the one without versions.
/// </para>
/// <para>
/// The property is the one that reads and writes one parameter, which is how the parameter is found. A command that is copied (<c>Clone</c>) is not
/// given defaults: it has what the original has, a parameter that is null in a file too.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public abstract class EditorDefaultBaseAttribute(StorageVersion[] versions) : Attribute
{
    /// <summary>The makers that the value is of; none means all of them.</summary>
    public IReadOnlyList<StorageVersion> Versions { get; } = versions;

    /// <summary>Makes the value for the property; objects (a tone, a list) are new ones of the type map of the maker.</summary>
    public abstract object? CreateValue(TypeMap map, Type propertyType);
}
