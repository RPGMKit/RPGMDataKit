using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MovePlaySeCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.PlaySe;
    public override string GetCommandName(StorageVersion version) => "SE";

    public ISE SoundData
    {
        get => GetEntity<ISE>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}