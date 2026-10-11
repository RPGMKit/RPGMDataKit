using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages;
using RubyMarshalCS.Serialization.Attributes;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

/// <summary>Base of VXAce entities that hold a list of commands (<see cref="ICommandContainer"/>).</summary>
/// <typeparam name="TCommand">Command interface; the class is taken from <see cref="TypeMap.VXAce"/>.</typeparam>
public abstract class RubyCommandContainer<TCommand> : RubySerializationEntity where TCommand : class, ICommand
{
    private readonly CommandList<TCommand> _commands = new(StorageVersion.VXAce, TypeMap.VXAce);

    /// <summary>The commands as stored in the file; only used by the serializer.</summary>
    [RubyProperty("@list")]
    protected List<TCommand>? RawList
    {
        get => _commands.ToRaw();
        set => _commands.LoadRaw(value);
    }

    public IList<NamedCommand> NamedCommands
    {
        get => _commands;
        set => _commands.Assign(value);
    }
}
