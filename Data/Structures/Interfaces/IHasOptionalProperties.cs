using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Structures.Exceptions;

namespace RPGMDataKit.Data.Structures.Interfaces;

/// <summary>
/// An entity some of whose properties one of the makers does not have: the class of that maker declares them (the interface has them) and marks
/// them with <see cref="DoesNotExistInVersionAttribute"/>, and they throw <see cref="DoesNotExistInVersionException"/>.
/// </summary>
public interface IHasOptionalProperties
{
    /// <summary>
    /// False if the property is one that the maker of the entity does not have. The answer is found once for the class.
    /// <code>if (sound.HasProperty(nameof(sound.Pan))) sound.Pan = pan;   // VX Ace sounds have no pan</code>
    /// </summary>
    /// <exception cref="ArgumentException">The class of the entity has no such property.</exception>
    public bool HasProperty(string propertyName);
}
