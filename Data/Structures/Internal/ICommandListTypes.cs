using RPGMDataKit.Storages;

namespace RPGMDataKit.Data.Structures.Internal;

/// <summary>The type map of a list of commands, which tells the maker of the container (the list is generic by the raw command).</summary>
internal interface ICommandListTypes
{
    TypeMap Types { get; }
}
