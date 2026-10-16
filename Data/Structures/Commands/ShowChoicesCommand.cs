using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ShowChoicesCommand : NamedCommand
{
    public override int Code => CommandCodes.ShowChoices;
    public override string GetCommandName(StorageVersion version) => "Show Choices";
    
    public enum PositionType
    {
        Left = 0,
        Middle = 1,
        Right = 2,
    }

    protected const int DefaultChoiceNone = -1;

    /// <summary>What Cancel (the Cancel button, Escape) does.</summary>
    public enum CancelType
    {
        /// <summary>Cancel is not allowed.</summary>
        Disallow,

        /// <summary>Cancel works as one of the choices, <see cref="CancelChoice"/>.</summary>
        Choice,

        /// <summary>Cancel is a branch of its own (When Cancel).</summary>
        Branch,
    }

    // what is told to the command while it is made in code and does not know its maker yet; it gets the raw value when it is put into a list
    private CancelType? _pendingCancel;
    private int _pendingCancelChoice;
    private bool _cancelIsSet;
    private bool _positionIsSet;

    // the list stored in the raw command, so changing it changes the command
    public List<string> Choices
    {
        get => GetEntity<List<string>>(0);
        set => Set(0, value);
    }

    /// <summary>
    /// What Cancel does, the same for both makers. MV counts the choices from 0 and has -1 for Disallow and -2 for Branch, VX Ace counts
    /// them from 1 and has 0 for Disallow and 5 for Branch; this property takes that into account (<see cref="CancelChoiceIndex"/> is the raw value).
    /// Can be set before the command is put into a list, it then gets the value of the maker of the list. A command that is not told
    /// anything does not allow Cancel.
    /// </summary>
    public CancelType Cancel
    {
        get => IsDetached ? _pendingCancel ?? CancelType.Disallow : Read().Type;
        set => WriteCancel(value, Math.Max(CancelChoice, 0));
    }

    /// <summary>The choice (from 0) that Cancel stands for when <see cref="Cancel"/> is <see cref="CancelType.Choice"/>, -1 otherwise. Setting it makes Cancel a choice; a negative value is ignored.</summary>
    public int CancelChoice
    {
        get => IsDetached ? (_pendingCancel == CancelType.Choice ? _pendingCancelChoice : -1) : Read().Choice;
        set
        {
            if (value >= 0)
                WriteCancel(CancelType.Choice, value);
        }
    }

    /// <summary>
    /// The raw value of Cancel, which depends on the maker: see <see cref="Cancel"/>. Use that one unless the raw value is what is needed.
    /// </summary>
    [RawValue]
    public int CancelChoiceIndex
    {
        get => Get<int>(1);
        set
        {
            // only what is set before the command is in a list counts: putting it there sets all properties
            if (IsDetached)
            {
                _cancelIsSet = true;
                _pendingCancel = null;
            }

            Set(1, value);
        }
    }

    public bool IsCancelChoiceBranch => Cancel == CancelType.Branch;

    private (CancelType Type, int Choice) Read() => Version == StorageVersion.MV
        ? CancelChoiceIndex switch
        {
            -2 => (CancelType.Branch, -1),
            < 0 => (CancelType.Disallow, -1),
            var choice => (CancelType.Choice, choice),
        }
        : CancelChoiceIndex switch
        {
            5 => (CancelType.Branch, -1),
            >= 1 and <= 4 => (CancelType.Choice, CancelChoiceIndex - 1),
            _ => (CancelType.Disallow, -1),
        };

    private void WriteCancel(CancelType type, int choice)
    {
        if (IsDetached)
        {
            _cancelIsSet = true;
            _pendingCancel = type;
            _pendingCancelChoice = choice;
        }
        else
        {
            Set(1, Encode(type, choice, Version));
        }
    }

    private static int Encode(CancelType type, int choice, StorageVersion version) => (type, version) switch
    {
        (CancelType.Branch, StorageVersion.MV) => -2,
        (CancelType.Disallow, StorageVersion.MV) => -1,
        (CancelType.Choice, StorageVersion.MV) => choice,
        (CancelType.Branch, _) => 5,
        (CancelType.Choice, _) => choice + 1,
        _ => 0,
    };

    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        // VX Ace has the choices and the cancel only; the other parameters are of MV
        if (version == StorageVersion.VXAce && Raw.Parameters is { Count: > 2 } parameters)
            parameters.RemoveRange(2, parameters.Count - 2);

        // a command that was not told what Cancel does does not allow it
        if (_pendingCancel != null || !_cancelIsSet)
            Set(1, Encode(_pendingCancel ?? CancelType.Disallow, _pendingCancelChoice, version));

        _pendingCancel = null;

        // the window of a new Show Choices is on the right in the editor of MV
        if (version == StorageVersion.MV && !_positionIsSet)
            Position = PositionType.Right;
    }

    /// <summary>The choice that is selected when the window opens, from 0, -1 for none. MV only: VX Ace throws <c>DoesNotExistInVersionException</c>.</summary>
    public int DefaultChoiceIndex
    {
        get
        {
            RequireVersion(StorageVersion.MV, nameof(DefaultChoiceIndex));
            return Get<int>(2);
        }
        set
        {
            RequireVersion(StorageVersion.MV, nameof(DefaultChoiceIndex));
            Set(2, value);
        }
    }

    /// <summary>MV only: VX Ace throws <c>DoesNotExistInVersionException</c>.</summary>
    public PositionType Position
    {
        get
        {
            RequireVersion(StorageVersion.MV, nameof(Position));
            return Get<PositionType>(3);
        }
        set
        {
            RequireVersion(StorageVersion.MV, nameof(Position));
            if (IsDetached)
                _positionIsSet = true;

            Set(3, value);
        }
    }

    /// <summary>MV only: VX Ace throws <c>DoesNotExistInVersionException</c>.</summary>
    public BackgroundType Background
    {
        get
        {
            RequireVersion(StorageVersion.MV, nameof(Background));
            return Get<BackgroundType>(4);
        }
        set
        {
            RequireVersion(StorageVersion.MV, nameof(Background));
            Set(4, value);
        }
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var cancelChoice = Cancel == CancelType.Choice ? "#" + (CancelChoice + 1) : "-";

        // VX Ace has no window settings and no default choice
        if (context.Version != StorageVersion.MV)
            return string.Join(", ", Choices) + $", ({cancelChoice})";

        var background = Background switch
        {
            BackgroundType.Window => "Window",
            BackgroundType.Dim => "Dim",
            BackgroundType.Transparent => "Transparent",
            _ => throw new Exception($"Unknown background '{Background}'")
        };

        var position = Position switch
        {
            PositionType.Right => "Right",
            PositionType.Middle => "Middle",
            PositionType.Left => "Left",
            _ => throw new Exception($"Unknown position '{Position}'")
        };

        var defaultChoice = DefaultChoiceIndex switch
        {
            DefaultChoiceNone => "-",
            _ => "#" + (DefaultChoiceIndex + 1)
        };

        return string.Join(", ", Choices) + $", ({background}, {position}, {defaultChoice}, {cancelChoice})";
    }
}