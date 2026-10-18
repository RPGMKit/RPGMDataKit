using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class PlaySECommand : NamedCommand
{
    public override int Code => CommandCodes.PlaySe;
    public override string GetCommandName(StorageVersion version) => "Play SE";
        
    public ISE SoundData
    {
        get => GetEntity<ISE>(0);
        set => Set(0, value);
    }

    // the editor of VX Ace gives a new SE the volume 80 (the factory of that maker gives 100)
    protected override void OnBinding(StorageVersion version)
    {
        if (version != StorageVersion.VXAce || IsParameterSet(0))
            return;

        var sound = Map.New<ISE>();
        sound.Volume = 80;
        Set(0, sound);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}