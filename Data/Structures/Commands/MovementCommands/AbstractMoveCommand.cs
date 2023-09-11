using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands
{
    public abstract class AbstractMoveCommand : NamedCommand
    {
        protected override string GetStringPrefix()
        {
            return "◇";
        }
    }
}
