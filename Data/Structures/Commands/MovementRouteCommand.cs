using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories.MovementCommands;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class MovementRouteCommand : NamedCommand
{
    public override int Code => CommandCodes.MovementRouteCommand;
    public override string GetCommandName(StorageVersion version) => "                  ";
        
    public MovementRouteCommand()
    {
        IsSecondary = true;
    }
        
    private NamedCommand? _pendingMoveCommand; // set in code, before this command has a container
    private NamedCommand? _moveCommand;
    private IMoveCommand? _moveCommandRaw;

    // a view over the movement command stored in the raw command
    public NamedCommand MoveCommand
    {
        get
        {
            if (IsDetached && _pendingMoveCommand != null)
                return _pendingMoveCommand;

            var raw = GetEntity<IMoveCommand>(0);
            if (raw == null)
                return _pendingMoveCommand!;

            if (_moveCommand == null || !ReferenceEquals(_moveCommandRaw, raw) || !_moveCommand.MatchesRaw())
            {
                _moveCommand = MovementCommandFactory.Create(raw, Version);
                _moveCommand.Types = Types;
                _moveCommandRaw = raw;
            }

            return _moveCommand;
        }
        set
        {
            if (IsDetached)
            {
                _pendingMoveCommand = value;
                return;
            }

            if (value == null)
                return;

            IMoveCommand raw;
            if (value.IsDetached)
            {
                raw = Map.Factory<IMoveCommand>()();
                value.Types = Types;
                value.Bind(raw, Version);
            }
            else
            {
                raw = (IMoveCommand)value.Raw;
            }

            Set(0, raw);
            _moveCommand = value;
            _moveCommandRaw = raw;
            _pendingMoveCommand = null;
        }
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return MoveCommand.AsString(context, new DumpOptions() {AppendCommandCode = options.AppendCommandCode, WithIndent = false, ExtendedFormat = options.ExtendedFormat});
    }
}