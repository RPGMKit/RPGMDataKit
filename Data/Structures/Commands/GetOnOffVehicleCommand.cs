using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class GetOnOffVehicleCommand : NamedCommand
{
    public override int Code => CommandCodes.GetOnOffVehicle;
    public override string GetCommandName(StorageVersion version) => "Get on/off Vehicle";
}