using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands.Factories.MovementCommands;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

/// <summary>
/// A step of a movement route as the list of commands of an event shows it (the command 505): the editors write one after Set Movement Route for
/// each step of its route, and show the route from them.
/// </summary>
public class MovementRouteCommand : NamedCommand
{
    public override int Code => CommandCodes.MovementRouteCommand;
    public override string GetCommandName(StorageVersion version) => "                  ";

    public MovementRouteCommand()
    {
        IsSecondary = true;
    }

    private NamedCommand? _moveCommand;
    private IMoveCommand? _moveCommandRaw;

    /// <summary>
    /// A view over the movement command stored in the raw command. A command that is not in a list yet has no maker, and holds the command that was
    /// set as it is; it becomes the raw command of the maker when this one is put into a list.
    /// </summary>
    public NamedCommand MoveCommand
    {
        get
        {
            if (IsDetached)
                return Get<NamedCommand>(0)!;

            var raw = GetEntity<IMoveCommand>(0);
            if (raw == null)
                return null!;

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
                Set(0, value);

                return;
            }

            if (value != null)
                Attach(value);
        }
    }

    // OnBinding is before the parameters are completed, which read this property: the command is the raw command of the maker by then
    protected override void OnBinding(StorageVersion version)
    {
        if (Raw.Parameters?.Count > 0 && Raw.Parameters[0] is NamedCommand held)
            Attach(held);
    }

    /// <summary>Stores the movement command as the parameter: the raw command of the maker of this command.</summary>
    private void Attach(NamedCommand value)
    {
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
    }

    internal override void Materialize()
    {
        base.Materialize();
        MoveCommand?.Materialize();
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return MoveCommand.AsString(context, new DumpOptions() {AppendCommandCode = options.AppendCommandCode, WithIndent = false, ExtendedFormat = options.ExtendedFormat});
    }
}
