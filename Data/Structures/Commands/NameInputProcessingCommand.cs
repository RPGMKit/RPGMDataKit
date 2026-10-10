using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class NameInputProcessingCommand : NamedCommand
{
    public override int Code => CommandCodes.NameInputProcessing;
    public override string GetCommandName(StorageVersion version) => "Name Input Processing";
        
    public int ActorId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public int MaxCharacters
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var actorName = context.NameFormatter.FormatActor(ActorId, new()
        {
            BracketType = BracketType.None
        });

        var str = $"{actorName}, {MaxCharacters} character(s)";

        return str;
    }
}