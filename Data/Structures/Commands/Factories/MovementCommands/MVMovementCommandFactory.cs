using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Versions.MV.MovementCommands;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Factories.MovementCommands;

public static class MVMovementCommandFactory
{
    public static NamedCommand Create(IEventCommand command)
    {
        NamedCommand returnCommand;

        switch (command.Code)
        {
            case MovementRouteCommandCodes.Se:
                returnCommand = new MVMoveSeCommand();
                break;
            default:
                returnCommand = GenericMovementCommandFactory.Create(command);
                break;
        }

        returnCommand.ParameterProvider = command.ParameterProvider;
        returnCommand.Indent = command.Indent;

        return returnCommand;
    }
}
