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

    // a new command has an empty sound with what the factory of the maker gives to a new one (MV: volume 90, pitch 100, pan 0)
    protected override void OnBinding(StorageVersion version)
    {
        if (!IsParameterSet(0))
            Set(0, Map.New<ISE>());
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}