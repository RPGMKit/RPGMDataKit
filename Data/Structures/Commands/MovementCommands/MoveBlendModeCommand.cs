using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveBlendModeCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.BlendMode;
    public override string GetCommandName(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return "Blend Mode";
        else
            return "Change Blending";
    }
    
    public BlendMode BlendMode { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        BlendMode = parameterProvider.GetParameter<BlendMode>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, BlendMode);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        switch (BlendMode)
        {
            case BlendMode.Normal:
                return "Normal";
            case BlendMode.Add:
                return context.Version == StorageVersion.MV ? "Additive" : "Add";
            case BlendMode.Sub:
                return "Sub";
            default:
                return $"Unknown ({BlendMode})";
        }
    }
}