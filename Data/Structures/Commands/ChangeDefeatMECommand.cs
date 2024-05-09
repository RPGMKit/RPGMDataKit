using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeDefeatME : AbstractAudioCommand
{
    public override int Code => CommandCodes.ChangeDefeatME;

    public override string GetCommandName(StorageVersion version) => "Change Defeat ME";
}
