using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Interfaces;

public interface ICommandContainer<out T> : IDumpable
{
    /// <summary>The commands. Assigning replaces all of them.</summary>
    public IList<NamedCommand> NamedCommands { get; set; }
}
