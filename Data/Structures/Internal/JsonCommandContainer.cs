using System.Runtime.Serialization;
using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

/// <summary>Base of MV entities that hold a list of commands (<see cref="ICommandContainer"/>).</summary>
/// <typeparam name="TCommand">Command interface; the class is taken from <see cref="TypeMap.Mv"/>.</typeparam>
public abstract class JsonCommandContainer<TCommand> : JsonEntity where TCommand : class, ICommand
{
    private readonly CommandList<TCommand> _commands = new(StorageVersion.MV, TypeMap.Mv);

    // the serializer of a storage passes its type map in the context, so commands made in code use the same one
    [OnDeserializing]
    private void TakeTypeMap(StreamingContext context)
    {
        if (context.Context is TypeMap types)
            _commands.Types = types;
    }

    /// <summary>The commands as stored in the file; only used by the serializer.</summary>
    [JsonProperty("list", ObjectCreationHandling = ObjectCreationHandling.Replace)]
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
