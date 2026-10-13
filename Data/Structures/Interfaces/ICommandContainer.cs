using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Interfaces;

public interface ICommandContainer : IDumpable
{
    /// <summary>The commands. Assigning replaces all of them.</summary>
    public IList<NamedCommand> NamedCommands { get; set; }
}
