using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class SetMovementRouteCommand : NamedCommand
{
    public override int Code => CommandCodes.SetMovementRoute;
    public override string GetCommandName(StorageVersion version) => "Set Movement Route";

    public int EventId
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    // the route stored in the raw command, so changing it changes the command
    public IMoveRoute Route
    {
        get => GetEntity<IMoveRoute>(1);
        set => Set(1, value);
    }

    public IList<NamedCommand> Commands => Route.NamedCommands;

    protected override void OnBinding(StorageVersion version)
    {
        // a new command is for the player, and its route is empty (the empty command that ends every route), not repeated, not skipped, waited for
        if (!IsParameterSet(0))
            Set(0, PlayerEventId);

        if (!IsParameterSet(1))
        {
            var route = Map.New<IMoveRoute>();
            route.Repeat = false;
            route.Skippable = false;
            route.Wait = true;
            route.NamedCommands = [new MovementCommands.MoveEmptyCommand()];
            Set(1, route);
        }
    }

    /// <summary>The number of the event for the player in a route; 0 is the event that runs the commands.</summary>
    public const int PlayerEventId = -1;

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatEvent(EventId);

        var parts = new List<string>();
        if (Route.Repeat)
            parts.Add("Repeat");

        if (Route.Skippable)
            parts.Add("Skip");

        if (Route.Wait)
            parts.Add("Wait");

        if (parts.Count > 0)
            str += " (" + string.Join(", ", parts) + ")";

        return str;
    }
}