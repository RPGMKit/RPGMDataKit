using RPGMakerLibCoreV2.Data.Structures.Commands.Factories.MovementCommands;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Classes;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Versions.VXAce
{
    public class VXAceMovementRouteCommand : MovementRouteCommand
    {
        public override NamedCommand Factory()
        {
            return VXAceMovementCommandFactory.Create(GetParameter<EventCommand>(0));
        }
    }
}
