using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Abstraction;

public abstract class AbstractEnableAccessCommand : NamedCommand
{
    public enum State
    {
        Disable = 0,
        Enable = 1,
    }

    public State GetState() => (State)GetParameter<int>(0);
    
    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return GetState() switch
        {
            State.Disable => "Disable",
            State.Enable => "Enable",
            _ => throw new Exception($"Unknown state {GetState()} for {GetCommandName(context.Version)} command")
        };
    }
}