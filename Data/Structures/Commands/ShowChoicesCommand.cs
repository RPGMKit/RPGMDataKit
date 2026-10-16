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

    // what the cancel value of MV is (the choices are counted from 0)
    private const int MvCancelBranch = -2;
    private const int MvCancelDisallow = -1;

    // what the cancel value of VX Ace is (the choices are counted from 1)
    private const int VxCancelBranch = 5;
    private const int VxCancelDisallow = 0;

    private enum CancelKind
    {
        Disallow,
        Branch,
    }

    // set while the command is made in code and does not know its version yet
    private CancelKind? _pendingCancel;
    private bool _cancelIsSet;
    private bool _positionIsSet;

    // the list stored in the raw command, so changing it changes the command
    public List<string> Choices
    {
        get => GetEntity<List<string>>(0);
        set => Set(0, value);
    }

    /// <summary>
    /// The raw value of what Cancel does, which differs between the makers: MV counts the choices from 0 and has -1 for Disallow and -2 for
    /// Branch, VX Ace counts them from 1 and has 0 for Disallow and 5 for Branch. To set Disallow or Branch for any maker use
    /// <see cref="SetCancelDisallow"/> and <see cref="SetCancelBranch"/>.
    /// </summary>
    public int CancelChoiceIndex
    {
        get => Get<int>(1);
        set
        {
            // only what is set before the command is in a list counts: putting it there sets all properties
            if (IsDetached)
                _cancelIsSet = true;

            Set(1, value);
        }
    }

    /// <summary>Cancel is not allowed. Can be called before the command is put into a list, it then gets the value of the maker of the list.</summary>
    public void SetCancelDisallow() => SetCancel(CancelKind.Disallow);

    /// <summary>Cancel is a branch of its own (When Cancel). Can be called before the command is put into a list, it then gets the value of the maker of the list.</summary>
    public void SetCancelBranch() => SetCancel(CancelKind.Branch);

    private void SetCancel(CancelKind kind)
    {
        if (IsDetached)
        {
            _cancelIsSet = true;
            _pendingCancel = kind;
        }
        else
        {
            CancelChoiceIndex = RawCancel(kind, Version);
        }
    }

    private static int RawCancel(CancelKind kind, StorageVersion version) => (kind, version) switch
    {
        (CancelKind.Branch, StorageVersion.MV) => MvCancelBranch,
        (CancelKind.Disallow, StorageVersion.MV) => MvCancelDisallow,
        (CancelKind.Branch, _) => VxCancelBranch,
        _ => VxCancelDisallow,
    };

    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        // VX Ace has the choices and the cancel only; the other parameters are of MV
        if (version == StorageVersion.VXAce && Raw.Parameters is { Count: > 2 } parameters)
            parameters.RemoveRange(2, parameters.Count - 2);

        // a command that was not told what Cancel does does not allow it, as the editors make it
        if (_pendingCancel != null || !_cancelIsSet)
            CancelChoiceIndex = RawCancel(_pendingCancel ?? CancelKind.Disallow, version);

        _pendingCancel = null;

        // the window of a new Show Choices is on the right in the editor of MV
        if (version == StorageVersion.MV && !_positionIsSet)
            Position = PositionType.Right;
    }

    public int DefaultChoiceIndex
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    public PositionType Position
    {
        get => Get<PositionType>(3);
        set
        {
            if (IsDetached)
                _positionIsSet = true;

            Set(3, value);
        }
    }

    public BackgroundType Background
    {
        get => Get<BackgroundType>(4);
        set => Set(4, value);
    }

    public bool IsCancelChoiceBranch => IsDetached
        ? _pendingCancel == CancelKind.Branch
        : CancelChoiceIndex == (Version == StorageVersion.MV ? MvCancelBranch : VxCancelBranch);

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
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

        var cancelChoice = CancelChoiceIndex switch
        {
            MvCancelBranch or MvCancelDisallow => "-",
            _ => "#" + (CancelChoiceIndex + 1)
        };

        return string.Join(", ", Choices) + $", ({background}, {position}, {defaultChoice}, {cancelChoice})";
    }
}