using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Versions.MV;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Factories;

public static class MVCommandFactory
{
    public static NamedCommand Create(IEventCommand command)
    {
        NamedCommand returnCommand;
        switch (command.Code)
        {
            case CommandCodes.MovementRouteCommand:
                returnCommand = new MVMovementRouteCommand();
                break;
            case CommandCodes.PlaySe:
                returnCommand = new MVPlaySECommand();
                break;
            case CommandCodes.ChangePartyMember:
                returnCommand = new MVChangePartyMemberCommand();
                break;
            case CommandCodes.If:
                returnCommand = MVIfFactory.Create(command.ParameterProvider.GetParameter<int>(0));
                break;
            default:
                returnCommand = GenericCommandFactory.Create(command);
                break;
        }

        returnCommand.ParameterProvider = command.ParameterProvider;
        returnCommand.Indent = command.Indent;

        return returnCommand;
    }
}
