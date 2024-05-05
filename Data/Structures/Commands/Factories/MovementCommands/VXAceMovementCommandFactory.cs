using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Versions.VXAce.MovementCommands;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Factories.MovementCommands;

public static class VXAceMovementCommandFactory
{
    public static NamedCommand Create(IEventCommand command)
    {
        NamedCommand returnCommand;

        switch (command.Code)
        {
            case MovementRouteCommandCodes.Se:
                returnCommand = new VXAceMoveSeCommand();
                break;
            default:
                returnCommand = GenericMovementCommandFactory.Create(command);
                break;
        }

        returnCommand.ParameterProvider = command.ParameterProvider;
        returnCommand.Indent = command.Indent;
        
        returnCommand.PrefixSymbol = "@>";
        returnCommand.SecondaryPrefixSymbol = ": ";

        return returnCommand;
    }
}
