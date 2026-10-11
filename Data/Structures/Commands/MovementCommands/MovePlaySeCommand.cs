using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MovePlaySeCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.PlaySe;
    public override string GetCommandName(StorageVersion version) => "SE";

    public ISE SoundData
    {
        get => GetEntity<ISE>(0);
        set => Set(0, IsDetached ? value : AudioFiles.For(value, Map));
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}