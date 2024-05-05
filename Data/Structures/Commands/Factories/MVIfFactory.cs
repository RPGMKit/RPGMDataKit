using RPGMakerLibCoreV2.Data.Structures.Commands.Versions.MV.IfCommands;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Factories;

public static class MVIfFactory
{
    public static IfCommand Create(int ifType)
    {
        IfCommand command;

        switch (ifType)
        {
            case GenericIfFactory.TYPE_VARIABLE:
                command = new MVIfVariableCommand();
                break;
            default:
                command = GenericIfFactory.Create(ifType);
                break;
        }

        return command;
    }
}
