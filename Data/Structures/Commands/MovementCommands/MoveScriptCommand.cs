using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveScriptCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Script;
    public override string GetCommandName(StorageVersion version) => "Script";

    public string Script
    {
        get => Get<string>(0);
        set => Set(0, value);
    }

    // a new command has an empty script, not none
    protected override void OnBinding(StorageVersion version)
    {
        if (!IsParameterSet(0))
            Set(0, "");
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options) => Script;
}