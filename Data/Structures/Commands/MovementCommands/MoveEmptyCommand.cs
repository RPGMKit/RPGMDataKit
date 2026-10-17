using RPGMDataKit.Data.Enums;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveEmptyCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.End;
    public override string GetCommandName(StorageVersion version) => "Empty";

    // the end of a route in the route of a Set Movement Route is {"code":0} in the files of MV: no parameters at all, not an empty list
    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            Raw.Parameters = null;
    }

    // it has no parameters to read, and an absent list is not to become an empty one (the file has {"code":0} or {"code":0,"parameters":[]})
    internal override void Materialize()
    {
    }
}
