using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class PlayMECommand : NamedCommand
{
    public override int Code => CommandCodes.PlayMe;
    public override string GetCommandName(StorageVersion version) => "Play ME";
    
    public IME SoundData
    {
        get => GetEntity<IME>(0);
        set => Set(0, IsDetached ? value : AudioFiles.For(value, Map));
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}