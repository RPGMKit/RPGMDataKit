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

    /// <summary>The volume that the editors give to the sound of a new command of a route: 90 in MV, 80 in VX Ace.</summary>
    public static int DefaultVolume(StorageVersion version) => version == StorageVersion.MV ? 90 : 80;

    // a new command has an empty sound with the volume of the editor and what the factory of the maker gives (the pitch 100, in MV the pan 0)
    protected override void OnBinding(StorageVersion version)
    {
        if (IsParameterSet(0))
            return;

        var sound = Map.New<ISE>();
        sound.Volume = DefaultVolume(version);
        Set(0, sound);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}