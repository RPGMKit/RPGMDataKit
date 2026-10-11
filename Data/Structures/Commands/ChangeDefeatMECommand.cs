using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeDefeatME : NamedCommand
{
    public override int Code => CommandCodes.ChangeDefeatME;

    public override string GetCommandName(StorageVersion version) => "Change Defeat ME";
    
    public IME SoundData
    {
        get => GetEntity<IME>(0);
        set => Set(0, value);
    }

    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        if (version != StorageVersion.MV)
            throw new Exception("Exists only in MV");
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }
}
