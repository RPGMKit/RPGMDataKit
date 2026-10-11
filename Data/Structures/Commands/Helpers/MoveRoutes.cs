using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Storages;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;

internal static class MoveRoutes
{
    /// <summary>
    /// The route as the type map stores <see cref="IMoveRoute"/>: <paramref name="route"/> itself when it already is,
    /// else a new one with the same settings and commands (e.g. a route of the other maker version).
    /// </summary>
    public static IMoveRoute? For(IMoveRoute? route, TypeMap map)
    {
        if (route == null)
            return null;

        if (map.Resolve(typeof(IMoveRoute)).IsInstanceOfType(route))
            return route;

        var converted = map.Factory<IMoveRoute>()();
        converted.Repeat = route.Repeat;
        converted.Skippable = route.Skippable;
        converted.Wait = route.Wait;
        converted.NamedCommands = route.NamedCommands;

        return converted;
    }
}
