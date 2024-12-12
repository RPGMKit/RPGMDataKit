using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class EnemyAppearCommand : NamedCommand
{
    public override int Code => CommandCodes.EnemyAppear;
        
    public override string GetCommandName(StorageVersion version) => "Enemy Appear";

    public int TroopId { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        TroopId = parameterProvider.GetParameter<int>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, TroopId);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return context.NameFormatter.FormatTroopMember(TroopId);
    }
}