using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeEncounterCommand : AbstractEnableAccessCommand
{
    public override int Code => CommandCodes.ChangeEncounter;
    public override string GetCommandName(StorageVersion version) => "Change Encounter";
}