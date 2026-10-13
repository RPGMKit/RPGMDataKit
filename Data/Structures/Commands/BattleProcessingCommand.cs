using System.Security.Cryptography;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

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
        
    public DesignationType DsgType
    {
        get => Get<DesignationType>(0);
        set => Set(0, value);
    }

    public int SourceId
    {
        get => Get<int>(1);
        set => Set(1, value);
    }

    public bool CanEscape
    {
        get => Get<bool>(2);
        set => Set(2, value);
    }

    public bool CanLoose
    {
        get => Get<bool>(3);
        set => Set(3, value);
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