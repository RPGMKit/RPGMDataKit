using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class NameInputProcessingCommand : NamedCommand
{
    public override int Code => CommandCodes.NameInputProcessing;
    public override string GetCommandName(StorageVersion version) => "Name Input Processing";
        
    [EditorDefault(1, StorageVersion.VXAce)]
    public int ActorId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    [EditorDefault(6, StorageVersion.VXAce)]
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