using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands;

public class IfEnemyCommand : IfCommand
{
    public enum CheckType
    {
        Appeared = 0,
        State = 1,
    }

    public int TroopId { get; set; }
    public CheckType Check { get; set; }
    public int Param { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Unpack(parameterProvider, version);

        TroopId = parameterProvider.GetParameter<int>(1);
        Check = (CheckType)parameterProvider.GetParameter<int>(2);
        Param = parameterProvider.GetParameter<int>(3);  
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        base.Pack(parameterProvider, version);
        
        parameterProvider.SetParameter(1, TroopId);
        parameterProvider.SetParameter(2, (int)Check);
        parameterProvider.SetParameter(3, Param);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = context.NameFormatter.FormatTroopMember(TroopId);

        switch (Check)
        {
            case CheckType.Appeared:
                if (context.Version == StorageVersion.MV)
                    str += " is appeared";
                str += " is Appeared";
                break;
            case CheckType.State:
                if (context.Version == StorageVersion.MV)
                    str += $" is affected by {context.NameFormatter.FormatState(Param)}";
                else
                    str += $" is {context.NameFormatter.FormatState(Param)} Inflicted";
                break;
            default:
                throw new Exception(
                    $"Unknown check type '{Check}' for '{GetCommandName(context.Version)}' enemy command");
        }

        return str;
    }
}