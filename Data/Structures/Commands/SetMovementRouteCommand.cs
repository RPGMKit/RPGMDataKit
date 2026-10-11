using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

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
        set => Set(1, IsDetached ? value : RouteFor(value));
    }

    public IList<NamedCommand> Commands => Route.NamedCommands;

    // the route as the version of the command stores it
    private IMoveRoute? RouteFor(IMoveRoute? route)
    {
        if (route == null)
            return null;

        if (Map.Resolve(typeof(IMoveRoute)).IsInstanceOfType(route))
            return route;

        var converted = Map.Factory<IMoveRoute>()();
        converted.Repeat = route.Repeat;
        converted.Skippable = route.Skippable;
        converted.Wait = route.Wait;
        converted.NamedCommands = route.NamedCommands;

        return converted;
    }

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