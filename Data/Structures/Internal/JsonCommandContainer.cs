using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

/// <summary>Base of MV entities that hold a list of commands (<see cref="ICommandContainer{T}"/>).</summary>
/// <typeparam name="TCommand">Command interface; the class is taken from <see cref="TypeMap.Mv"/>.</typeparam>
public abstract class JsonCommandContainer<TCommand> : JsonEntity where TCommand : class, ICommand
{
    private static readonly Func<TCommand> CreateCommand = TypeMap.Mv.Factory<TCommand>();

    private readonly CommandList<TCommand> _commands = new(StorageVersion.MV, CreateCommand);

    [JsonProperty("list")]
    public List<TCommand>? List
    {
        get => _commands.Raw;
        set => _commands.Raw = value;
    }

    public List<NamedCommand> NamedCommands
    {
        get => _commands.Named;
        set => _commands.Named = value;
    }

    public IEnumerable<TCommand>? Commands => List;
}
