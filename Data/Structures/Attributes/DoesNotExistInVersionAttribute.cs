using RPGMDataKit.Data.Structures.Exceptions;

namespace RPGMDataKit.Data.Structures.Attributes;

/// <summary>
/// Marks a property of a class of a maker that the maker does not have. The property is declared because the interface has it, and it throws
/// <see cref="DoesNotExistInVersionException"/>; <c>HasProperty</c> (<see cref="Helpers.EntityExtensions"/>) tells that without calling it.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DoesNotExistInVersionAttribute : Attribute;
