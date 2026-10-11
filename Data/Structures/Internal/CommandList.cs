using System.Collections.ObjectModel;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

/// <summary>
/// Commands of a container. Users work with <see cref="NamedCommand"/>s only: each of them is a view over its raw
/// command, so editing one edits the raw data. The raw commands are only used to read and write the file
/// (<see cref="LoadRaw"/>, <see cref="ToRaw"/>).
/// </summary>
public sealed class CommandList<TCommand> : Collection<NamedCommand> where TCommand : class, ICommand
{
    private readonly StorageVersion _version;

    // a list that is absent in the file stays absent when saved, until a command is added
    private bool _isNull = true;

    /// <param name="types">Where the raw commands of commands made in code come from; replaced by the one of the storage.</param>
    public CommandList(StorageVersion version, TypeMap types)
    {
        _version = version;
        Types = types;
    }

    /// <summary>The type map of the storage that the container belongs to.</summary>
    public TypeMap Types { get; set; }

    /// <summary>Builds the named commands over the raw commands that were read from the file.</summary>
    public void LoadRaw(List<TCommand>? raw)
    {
        ClearItems();
        _isNull = raw == null;

        if (raw == null)
            return;

        foreach (var command in raw)
        {
            var named = CommandContainerHelper.CreateNamed(command, _version);
            named.Types = Types;
            named.Owner = this;
            base.InsertItem(Count, named);
        }
    }

    /// <summary>The raw commands to write to the file.</summary>
    public List<TCommand>? ToRaw()
    {
        return _isNull ? null : this.Select(named => (TCommand)named.Raw).ToList();
    }

    /// <summary>Makes every command read all of its parameters, so objects in them are of their classes.</summary>
    public void Materialize()
    {
        foreach (var command in this)
            command.Materialize();
    }

    /// <summary>Replaces all commands.</summary>
    public void Assign(IEnumerable<NamedCommand> commands)
    {
        var items = commands.ToList(); // the source may be this very list

        Clear();
        _isNull = false;
        foreach (var command in items)
            Add(command);
    }

    protected override void InsertItem(int index, NamedCommand item)
    {
        var attached = Attach(item);
        _isNull = false;
        base.InsertItem(index, attached);
    }

    protected override void SetItem(int index, NamedCommand item)
    {
        var old = this[index];
        if (ReferenceEquals(old, item))
            return;

        var attached = Attach(item);
        old.Owner = null;
        base.SetItem(index, attached);
    }

    protected override void RemoveItem(int index)
    {
        this[index].Owner = null;
        base.RemoveItem(index);
    }

    protected override void ClearItems()
    {
        foreach (var command in this)
            command.Owner = null;

        base.ClearItems();
    }

    /// <summary>
    /// A command made in code gets its raw command when it is put into the list. A command of a list cannot be put into
    /// another list (or twice into the same one): the raw command would belong to both, and one of the storages can be
    /// of another maker version.
    /// </summary>
    private NamedCommand Attach(NamedCommand item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (item.Owner != null)
        {
            throw new InvalidOperationException(item.Owner == this
                ? "The command is already in this list."
                : "The command is in another list, it cannot be in two. Remove it from there or use Clone().");
        }

        if (item.IsDetached)
        {
            item.Types = Types;
            item.Bind(Types.Factory<TCommand>()(), _version);
        }

        item.Owner = this;

        return item;
    }
}
