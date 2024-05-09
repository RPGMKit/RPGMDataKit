using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

public abstract class AbstractEnableAccessCommand : NamedCommand
{
    public bool GetIsOn() => GetParameter<int>(0) == 1;

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return GetIsOn() ? "Enable" : "Disable";
    }
}