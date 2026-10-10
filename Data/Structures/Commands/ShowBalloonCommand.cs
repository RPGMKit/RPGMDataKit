using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShowBalloonCommand : NamedCommand
{
    public override int Code => CommandCodes.ShowBaloonIcon;
    public override string GetCommandName(StorageVersion version) => "Show Balloon Icon";
    
    public enum IconType
    {
        Exclamation = 1,
        Question = 2,
        MusicNote = 3,
        Heart = 4,
        Anger = 5,
        Sweat = 6,
        Cobweb = 7,
        Silence = 8,
        LightBulb = 9,
        Zzz = 10,
        UserDefined1 = 11,
        UserDefined2 = 12,
        UserDefined3 = 13,
        UserDefined4 = 14,
        UserDefined5 = 15,
    }

    public int EventId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public IconType Icon
    {
        get => Get<IconType>(1);
        set => Set(1, value);
    }

    public bool WaitForCompletion
    {
        get => Get<bool>(2);
        set => Set(2, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatEvent(EventId);

        str += ", ";

        str += Icon switch
        {
            IconType.Exclamation => "Exclamation",
            IconType.Question => "Question",
            IconType.MusicNote => "Music Note",
            IconType.Heart => "Heart",
            IconType.Anger => "Anger",
            IconType.Sweat => "Sweat",
            IconType.Cobweb => "Cobweb",
            IconType.Silence => "Silence",
            IconType.LightBulb => "Light Bulb",
            IconType.Zzz => "Zzz",
            IconType.UserDefined1 => "User-defined 1",
            IconType.UserDefined2 => "User-defined 2",
            IconType.UserDefined3 => "User-defined 3",
            IconType.UserDefined4 => "User-defined 4",
            IconType.UserDefined5 => "User-defined 5",
            _ => throw new Exception($"Unknown balloon icon '{Icon}'")
        };

        if (WaitForCompletion)
            str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

        return str;
    }
}