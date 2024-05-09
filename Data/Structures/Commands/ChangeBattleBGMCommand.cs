using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeBattleBGMCommand : AbstractAudioCommand
{
    public override int Code => CommandCodes.ChangeBattleBGM;
    public override string GetCommandName(StorageVersion version) => "Change Battle BGM";
}
