using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Versions.VXAce;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Factories;

public static class VXAceCommandFactory
{
    public static NamedCommand Create(IEventCommand command)
    {
        NamedCommand returnCommand;
        switch (command.Code)
        {
            case CommandCodes.MovementRouteCommand:
                returnCommand = new VXAceMovementRouteCommand();
                break;
            case CommandCodes.PlaySe:
                returnCommand = new VXAcePlaySECommand();
                break;
            case CommandCodes.ChangePartyMember:
                returnCommand = new VXAceChangePartyMemberCommand();
                break;
            default:
                returnCommand = GenericCommandFactory.Create(command);
                break;
        }

        returnCommand.ParameterProvider = command.ParameterProvider;
        returnCommand.Indent = command.Indent;

        returnCommand.PrefixSymbol = "@>";
        returnCommand.SecondaryPrefixSymbol = ": ";

        return returnCommand;
    }
}
