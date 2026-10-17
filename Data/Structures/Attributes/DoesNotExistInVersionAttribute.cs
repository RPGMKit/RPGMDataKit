using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Exceptions;

namespace RPGMDataKit.Data.Structures.Attributes;

/// <summary>
/// Marks a property that a maker does not have, which the property says by throwing <see cref="DoesNotExistInVersionException"/>;
/// <c>HasProperty</c> (<see cref="Interfaces.IHasOptionalProperties"/>) tells that without calling it.
/// <list type="bullet">
/// <item>On a property of a command, whose class is the same for both makers: the makers that do not have it,
/// <c>[DoesNotExistInVersion(StorageVersion.VXAce)]</c> (the position of the choices is MV only). A command that is not in a list has no maker yet, and has the property.</item>
/// <item>On a property of an entity, whose class is of one maker (such as the pan of a sound in the class of VX Ace): without versions,
/// the class does not have the property.</item>
/// </list>
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DoesNotExistInVersionAttribute(params StorageVersion[] versions) : Attribute
{
    /// <summary>The makers that do not have the property; none means that the class itself does not have it.</summary>
    public IReadOnlyList<StorageVersion> Versions { get; } = versions;
}
