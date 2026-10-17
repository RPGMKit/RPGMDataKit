using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Internal;
using RPGMDataKit.Data.Structures.Attributes;

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

    /// <summary>
    /// What Cancel (the Cancel button, Escape) does. The numbers are the ones of MV, so a command that is not in a list yet, and so does not know
    /// its maker, keeps the value as it is, and it is changed into the number of VX Ace when the command is put into a list of VX Ace.
    /// </summary>
    public enum CancelType
    {
        /// <summary>Cancel is not allowed.</summary>
        Disallow = -1,

        /// <summary>Cancel is a branch of its own (When Cancel).</summary>
        Branch = -2,

        /// <summary>Cancel works as the first choice, and so on. MV has up to 6 choices, VX Ace up to 4.</summary>
        Choice1 = 0,
        Choice2 = 1,
        Choice3 = 2,
        Choice4 = 3,
        Choice5 = 4,
        Choice6 = 5,
    }

    // the list stored in the raw command, so changing it changes the command
    public List<string> Choices
    {
        get => GetEntity<List<string>>(0);
        set => Set(0, value);
    }

    private bool IsInVxAce => !IsDetached && Version == StorageVersion.VXAce;

    /// <summary>
    /// What Cancel does, the same for both makers. The raw value depends on the maker (MV: -1 not allowed, -2 a branch, the choices from 0;
    /// VX Ace: 0, 5, the choices from 1), this property takes it into account (<see cref="CancelChoiceIndex"/> is the raw value). Can be set
    /// before the command is put into a list. A command that is not told anything does not allow Cancel.
    /// </summary>
    public CancelType Cancel
    {
        get => IsInVxAce ? FromVxAce(Get<int>(1)) : (CancelType)Get<int>(1);
        set => Set(1, IsInVxAce ? ToVxAce(value) : (int)value);
    }

    /// <summary>
    /// The raw value of Cancel, which depends on the maker: see <see cref="Cancel"/>. Use that one unless the raw value is what is needed. For
    /// a command that is not in a list yet it is the number of MV (a command without a maker keeps what was set in code until it gets one).
    /// </summary>
    [RawValue]
    public int CancelChoiceIndex
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public bool IsCancelChoiceBranch => Cancel == CancelType.Branch;

    private static int ToVxAce(CancelType cancel) => cancel switch
    {
        CancelType.Disallow => 0,
        CancelType.Branch => 5,
        <= CancelType.Choice4 => (int)cancel + 1,
        _ => throw new DoesNotExistInVersionException($"{cancel} exists in MV only"),
    };

    private static CancelType FromVxAce(int raw) => raw switch
    {
        5 => CancelType.Branch,
        >= 1 and <= 4 => (CancelType)(raw - 1),
        _ => CancelType.Disallow,
    };

    protected override void OnBinding(StorageVersion version)
    {
        // a command that was not told what Cancel does does not allow it
        if (!IsParameterSet(1))
            Set(1, (int)CancelType.Disallow);

        // what was set in code is the number of MV until the command has a maker
        if (version == StorageVersion.VXAce)
            Set(1, ToVxAce((CancelType)Get<int>(1)));

        // the window of a new Show Choices is on the right in the editor of MV
        if (version == StorageVersion.MV && !IsParameterSet(3))
            Set(3, PositionType.Right);
    }

    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        // VX Ace has the choices and the cancel only; the other parameters are of MV
        if (version == StorageVersion.VXAce && Raw.Parameters is { Count: > 2 } parameters)
            parameters.RemoveRange(2, parameters.Count - 2);
    }

    /// <summary>The choice that is selected when the window opens, from 0, -1 for none. MV only: VX Ace throws <c>DoesNotExistInVersionException</c>.</summary>
    [DoesNotExistInVersion(StorageVersion.VXAce)]
    public int DefaultChoiceIndex
    {
        get => Get<int>(2);
        set => Set(2, value);
    }

    /// <summary>MV only: VX Ace throws <c>DoesNotExistInVersionException</c>.</summary>
    [DoesNotExistInVersion(StorageVersion.VXAce)]
    public PositionType Position
    {
        get => Get<PositionType>(3);
        set => Set(3, value);
    }

    /// <summary>MV only: VX Ace throws <c>DoesNotExistInVersionException</c>.</summary>
    [DoesNotExistInVersion(StorageVersion.VXAce)]
    public BackgroundType Background
    {
        get => Get<BackgroundType>(4);
        set => Set(4, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var cancelChoice = Cancel >= CancelType.Choice1 ? "#" + ((int)Cancel + 1) : "-"; // the choice, not a branch or nothing

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