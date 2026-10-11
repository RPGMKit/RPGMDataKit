using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class PlayBGMCommand : NamedCommand
{
    public override int Code => CommandCodes.PlayBgm;
    public override string GetCommandName(StorageVersion version) => "Play BGM";


    public IBGM SoundData
    {
        get => GetEntity<IBGM>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatSoundData(SoundData);
    }

}