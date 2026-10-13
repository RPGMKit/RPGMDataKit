using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.Abstraction;

public abstract class AbstractEnableAccessCommand : NamedCommand
{
    public DisableEnableState State
    {
        get => Get<DisableEnableState>(0);
        set => Set(0, value);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return State switch
        {
            DisableEnableState.Disable => "Disable",
            DisableEnableState.Enable => "Enable",
            _ => throw new Exception($"Unknown state {State} for {GetCommandName(context.Version)} command")
        };
    }
}