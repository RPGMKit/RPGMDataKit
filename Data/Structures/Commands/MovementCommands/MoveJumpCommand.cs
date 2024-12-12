using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;

public class MoveJumpCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.Jump;
    public override string GetCommandName(StorageVersion version) => "Jump";
    
    public int XDelta  { get; set; }
    public int YDelta  { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        XDelta = parameterProvider.GetParameter<int>(0);
        YDelta = parameterProvider.GetParameter<int>(1);
    }
    
    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, XDelta);
        parameterProvider.SetParameter(1, YDelta);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"{XDelta:+#;-#;0}, {YDelta:+#;-#;0}";
    }
}