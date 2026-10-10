using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveSwitchOnCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.SwitchOn;
    public override string GetCommandName(StorageVersion version) => "Switch ON";

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