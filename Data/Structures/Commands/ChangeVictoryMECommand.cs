using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeVictoryMECommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeVictoryME;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Change Victory ME";
        else
            return "Change Battle End ME";
    }
    
    public IME SoundData
    {
        get => GetEntity<IME>(0);
        set => Set(0, value);
    }
        
    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}