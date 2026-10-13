using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class RecoverAllCommand : NamedCommand
{
    public override int Code => CommandCodes.RecoverAll;
    public override string GetCommandName(StorageVersion version) => "Recover All";
    
    public enum SourceType
    {
        Actor = 0,
        Variable = 1,
    }

    public SourceType Source
    {
        get => Get<SourceType>(0);
        set => Set(0, value);
    }

    public int SourceId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = Source switch
        {
            SourceType.Actor => context.NameFormatter.FormatActor(SourceId),
            SourceType.Variable => context.NameFormatter.FormatVariable(SourceId),
            _ => throw new Exception($"Unknown recover all source type '{Source}'")
        };

        return str;
    }
}