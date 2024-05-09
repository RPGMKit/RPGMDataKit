using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeVictoryMECommand : AbstractAudioCommand
{
    public override int Code => CommandCodes.ChangeVictoryME;

    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Change Victory ME";
        else
            return "Change Battle End ME";
    }
}