using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveSwitchOffCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.SwitchOff;
    public override string GetCommandName(StorageVersion version) => "Switch OFF";

    public int SwitchId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{SwitchId:D4}";
    }
}