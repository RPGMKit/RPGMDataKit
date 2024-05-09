using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

public abstract class AbstractOnOffCommand : NamedCommand
{
    public bool GetOn() => GetParameter<int>(0) == 0;

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return GetOn() ? "ON" : "OFF";
    }
}