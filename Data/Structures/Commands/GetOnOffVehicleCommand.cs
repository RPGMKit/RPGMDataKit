using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class GetOnOffVehicleCommand : NamedCommand
{
    public override int Code => CommandCodes.GetOnOffVehicle;
    public override string GetCommandName(StorageVersion version) => "Get on/off Vehicle";
}