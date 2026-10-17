using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveSwitchOnCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.SwitchOn;
    public override string GetCommandName(StorageVersion version) => "Switch ON";

    public int SwitchId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    // a new command is for the switch 1 in the editors
    protected override void OnBinding(StorageVersion version)
    {
        if (!IsParameterSet(0))
            Set(0, 1);
    }
    
    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{SwitchId:D4}";
    }
}