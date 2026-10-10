using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

/// <summary>
/// Commands of a container in two forms: raw commands as stored in the file and <see cref="NamedCommand"/>s.
/// Named commands are built on first access. Until then the raw list is kept untouched, so entities nobody
/// looked into are saved exactly as they were read.
/// </summary>
public class CommandList<TCommand> where TCommand : ICommand
{
    private readonly StorageVersion _version;
    private readonly Func<TCommand> _create;
    private List<TCommand>? _raw;
    private List<NamedCommand>? _named;

    /// <param name="create">Creates an empty raw command when named commands are packed.</param>
    public CommandList(StorageVersion version, Func<TCommand> create)
    {
        _version = version;
        _create = create;
    }

    /// <summary>Commands in the file form. Rebuilt from <see cref="Named"/> once those were accessed.</summary>
    public List<TCommand>? Raw
    {
        get => _named != null ? CommandContainerHelper.Pack(_named, _version, _create) : _raw;
        set
        {
            _raw = value;
            _named = null;
        }
    }

    public List<NamedCommand> Named
    {
        get => _named ??= CommandContainerHelper.Unpack(_raw, _version)!;
        set => _named = value;
    }
}
