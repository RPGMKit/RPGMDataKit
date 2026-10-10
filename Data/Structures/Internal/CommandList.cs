using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

/// <summary>
/// Commands of a container in two forms: raw commands as stored in the file and <see cref="NamedCommand"/>s.
/// Named commands are built on first access. Until then the raw list is kept untouched, so entities nobody
/// looked into are saved exactly as they were read.
/// </summary>
public class CommandList<TRaw> where TRaw : ICommand, new()
{
    private readonly StorageVersion _version;
    private List<TRaw>? _raw;
    private List<NamedCommand>? _named;

    public CommandList(StorageVersion version)
    {
        _version = version;
    }

    /// <summary>Commands in the file form. Rebuilt from <see cref="Named"/> once those were accessed.</summary>
    public List<TRaw>? Raw
    {
        get => _named != null ? CommandContainerHelper.Pack<TRaw>(_named, _version) : _raw;
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
