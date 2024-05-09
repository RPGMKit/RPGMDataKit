using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SetMovementRouteCommand : NamedCommand
    {
        public override int Code => CommandCodes.SetMovementRoute;
        public override string GetCommandName(StorageVersion version) => "Set Movement Route";

        public int GetSource()
        {
            return GetParameter<int>(0);
        }

        public IRoute GetRoute(StorageVersion version)
        {
            if (version == StorageVersion.MV)
                return GetParameter<Route>(1);

            return GetParameter<IRoute>(1);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var route = GetRoute(context.Version);
            var str = context.NameFormatter.FormatEvent(GetSource());

            var parts = new List<string>();
            if (route.Repeat)
                parts.Add("Repeat");

            if (route.Skippable)
                parts.Add("Skip");

            if (route.Wait)
                parts.Add("Wait");

            if (parts.Count > 0)
                str += " (" + string.Join(", ", parts) + ")";

            return str;
        }
    }
}

