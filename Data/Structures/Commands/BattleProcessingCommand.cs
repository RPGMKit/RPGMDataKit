using System.Security.Cryptography;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class BattleProcessingCommand : NamedCommand
{
    public override int Code => CommandCodes.BattleProcessing;
    public override string GetCommandName(StorageVersion version) => "Battle Processing";
        
    public enum DesignationType
    {
        Direct = 0,
        ByVariable = 1,
        SameAsRandomEncounter = 2,
    }
        
    public DesignationType DsgType { get; set; }
    public int SourceId { get; set; }
    public bool CanEscape { get; set; }
    public bool CanLoose { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion storageVersion)
    {
        DsgType = parameterProvider.GetParameter<DesignationType>(0);
        SourceId = parameterProvider.GetParameter<int>(1);
        CanEscape = parameterProvider.GetParameter<bool>(2);
        CanLoose = parameterProvider.GetParameter<bool>(3);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, DsgType);
        parameterProvider.SetParameter(1, SourceId);
        parameterProvider.SetParameter(2, CanEscape);
        parameterProvider.SetParameter(3, CanLoose);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = DsgType switch
        {
            DesignationType.Direct => context.NameFormatter.FormatTroop(SourceId),
            DesignationType.ByVariable => context.NameFormatter.FormatVariable(SourceId,
                new() { SkipName = context.Version != StorageVersion.MV }),
            DesignationType.SameAsRandomEncounter => "Same as Random Encounter",
            _ => throw new Exception($"Unknown battle processing designation type '{DsgType}'")
        };

        // custom
        if (CanEscape)
            str += ", Can escape";

        // custom
        if (CanLoose)
            str += ", Can lose";

        return str;
    }
}