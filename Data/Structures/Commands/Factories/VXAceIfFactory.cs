using RPGMakerLibCoreV2.Data.Structures.Commands.Versions.VXAce.IfCommand;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Factories;

public static class VXAceIfFactory
{
    public static IfCommand Create(int ifType)
    {
        IfCommand command;

        switch (ifType)
        {
            case GenericIfFactory.TYPE_VARIABLE:
                command = new VXAceIfVariableCommand();
                break;
            default:
                command = GenericIfFactory.Create(ifType);
                break;
        }

        command.NameProvider = "Conditional Branch";

        return command;
    }
}
