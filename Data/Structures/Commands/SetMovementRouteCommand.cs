using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

using MoveRouteVX = VXAce.Classes.MoveRoute;
using MoveRouteMV = MoveRoute;

public class SetMovementRouteCommand : NamedCommand
{
    public override int Code => CommandCodes.SetMovementRoute;
    public override string GetCommandName(StorageVersion version) => "Set Movement Route";

    public int EventId { get; set; }
    public IMoveRoute Route { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        EventId = parameterProvider.GetParameter<int>(0);
        Route = version == StorageVersion.MV
            ? parameterProvider.GetParameter<MoveRouteMV>(1)
            : parameterProvider.GetParameter<IMoveRoute>(1);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, EventId);

        IMoveRoute route = version == StorageVersion.MV ? new MoveRouteMV() : new MoveRouteVX();
        route.Repeat = Route.Repeat;
        route.Skippable = Route.Skippable;
        route.Wait = Route.Wait;
        
        parameterProvider.SetParameter(1, route);
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