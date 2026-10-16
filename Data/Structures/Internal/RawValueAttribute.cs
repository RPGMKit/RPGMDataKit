namespace RPGMDataKit.Data.Structures.Internal;

/// <summary>
/// Marks a property of a command that is the raw value of another property, which holds the same thing in a way that does not depend on the maker
/// (for example the raw cancel of Show Choices is -1/-2/n in MV and 0/5/n+1 in VX Ace). Copying a command and completing a command made in code
/// skip it: the raw value of one maker would take the place of the value that was made for the other.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
internal sealed class RawValueAttribute : Attribute;
