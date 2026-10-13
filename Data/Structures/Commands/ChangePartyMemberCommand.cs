using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangePartyMemberCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangePartyMember;
    public override string GetCommandName(StorageVersion version) => "Change Party Member";
        
    public enum ChangeType
    {
        Add = 0,
        Remove = 1,
    }

    public int ActorId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public ChangeType Type
    {
        get => Get<ChangeType>(1);
        set => Set(1, value);
    }

    // a bool in MV, 0 or 1 in VXAce
    public bool DoInitialize
    {
        get => Version == StorageVersion.MV ? Get<bool>(2) : Get<int>(2) > 0;
        set
        {
            if (Version == StorageVersion.MV)
                Set(2, value);
            else
                Set(2, value ? 1 : 0);
        }
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = Type switch
        {
            ChangeType.Add => "Add",
            ChangeType.Remove => "Remove",
            _ => throw new Exception($"Unknown add/remove state for party member")
        };

        str += context.NameFormatter.FormatActor(ActorId);
        if (DoInitialize)
            str += context.Version == StorageVersion.MV ? " (Initialize)" : ", Initialize";

        return str;
    }
}