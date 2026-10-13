using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands.Abstraction;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeEncounterCommand : AbstractEnableAccessCommand
{
    public override int Code => CommandCodes.ChangeEncounter;
    public override string GetCommandName(StorageVersion version) => "Change Encounter";
}