using System.Collections.ObjectModel;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

/// <summary>
/// Commands of a container. Users work with <see cref="NamedCommand"/>s only: each of them is a view over its raw
/// command, so editing one edits the raw data. The raw commands are only used to read and write the file
/// (<see cref="LoadRaw"/>, <see cref="ToRaw"/>).
/// </summary>
public sealed class CommandList<TCommand> : Collection<NamedCommand> where TCommand : class, ICommand
{
    private readonly StorageVersion _version;
    private readonly Func<TCommand> _create;

    // a list that is absent in the file stays absent when saved, until a command is added
    private bool _isNull = true;

    /// <param name="create">Creates an empty raw command for a named command made in code.</param>
    public CommandList(StorageVersion version, Func<TCommand> create)
    {
        _version = version;
        _create = create;
    }

    /// <summary>Builds the named commands over the raw commands that were read from the file.</summary>
    public void LoadRaw(List<TCommand>? raw)
    {
        base.ClearItems();
        _isNull = raw == null;

        if (raw == null)
            return;

        foreach (var command in raw)
            base.InsertItem(Count, CommandContainerHelper.CreateNamed(command, _version));
    }

    /// <summary>The raw commands to write to the file.</summary>
    public List<TCommand>? ToRaw()
    {
        return _isNull ? null : this.Select(named => (TCommand)named.Raw).ToList();
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
        _isNull = false;
        base.InsertItem(index, Attach(item));
    }

    protected override void SetItem(int index, NamedCommand item)
    {
        base.SetItem(index, Attach(item));
    }

    /// <summary>A command made in code gets its raw command when it is put into the list.</summary>
    private NamedCommand Attach(NamedCommand item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (item.IsDetached)
            item.Bind(_create(), _version);

        return item;
    }
}
