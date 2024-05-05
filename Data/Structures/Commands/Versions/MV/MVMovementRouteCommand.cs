using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories.MovementCommands;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Versions.MV
{
    public class MVMovementRouteCommand : MovementRouteCommand
    {
        public override NamedCommand Factory()
        {
            return MVMovementCommandFactory.Create(GetParameter<EventCommand>(0));
        }
    }
}
