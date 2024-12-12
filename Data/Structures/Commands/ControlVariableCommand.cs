using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ControlVariableCommand : NamedCommand
{
    public override int Code => CommandCodes.ControlVariables;
    public override string GetCommandName(StorageVersion version) => "Control Variables";
        
    public enum OperatorType
    {
        Set = 0,
        Add = 1,
        Dec = 2,
        Mul = 3,
        Div = 4,
        Mod = 5,
    }

    public enum OperandType
    {
        Constant = 0,
        Variable = 1,
        Random = 2,
        GameData = 3,
        Script = 4,
    }

    public enum GameDataType
    {
        ItemInInventory = 0,
        WeaponInInventory = 1,
        ArmorInInventory = 2,
        Actor = 3,
        Enemy = 4,
        Character = 5,
        Party = 6,
        Other = 7,
    }

    public enum ActorParameters
    {
        Level = 0,
        Exp = 1,
        Hp = 2,
        Mp = 3,
        MaxHp = 4,
        MaxMp = 5,
        Attack = 6,
        Defense = 7,
        MAttack = 8,
        MDefense = 9,
        Agility = 10,
        Luck = 11,
    }

    public enum EnemyParameters
    {
        Hp = 0,
        Mp = 1,
        MaxHp = 2,
        MaxMp = 3,
        Attack = 4,
        Defense = 5,
        MAttack = 6,
        MDefense = 7,
        Agility = 8,
        Luck = 9,
    }
        
    public enum CharacterParameters
    {
        MapX = 0,
        MapY = 1,
        Direction = 2,
        ScreenX = 3,
        ScreenY = 4,
    }
        
    public enum OtherParameters
    {
        MapId = 0,
        PartyMembers = 1,
        Gold = 2,
        Steps = 3,
        PlayTime = 4,
        Timer = 5,
        SaveCount = 6,
        BattleCount = 7,
        WinCount = 8,
        EscapeCount = 9,
    }

    private static readonly Dictionary<ActorParameters, Tuple<string, string>> GameDataActorParameters = new()
    {
        [ActorParameters.Level] = new("Level", "Level"),
        [ActorParameters.Exp] = new("EXP", "EXP"),
        [ActorParameters.Hp] = new("HP", "HP"),
        [ActorParameters.Mp] = new("MP", "MP"),
        [ActorParameters.MaxHp] = new("Max HP", "MHP"),
        [ActorParameters.MaxMp] = new("Max MP", "MMP"),
        [ActorParameters.Attack] = new("Attack", "ATK"),
        [ActorParameters.Defense] = new("Defense", "DEF"),
        [ActorParameters.MAttack] = new("M.Attack", "MAT"),
        [ActorParameters.MDefense] = new("M.Defense", "MDF"),
        [ActorParameters.Agility] = new("Agility", "AGI"),
        [ActorParameters.Luck] = new("Luck", "LUK"),
    };

    private static readonly Dictionary<EnemyParameters, Tuple<string, string>> GameDataEnemyParameters = new()
    {
        [EnemyParameters.Hp] = new("HP", "HP"),
        [EnemyParameters.Mp] = new("MP", "MP"),
        [EnemyParameters.MaxHp] = new("Max HP", "MHP"),
        [EnemyParameters.MaxMp] = new("Max MP", "MMP"),
        [EnemyParameters.Attack] = new("Attack", "ATK"),
        [EnemyParameters.Defense] = new("Defense", "DEF"),
        [EnemyParameters.MAttack] = new("M.Attack", "MAT"),
        [EnemyParameters.MDefense] = new("M.Defense", "MDF"),
        [EnemyParameters.Agility] = new("Agility", "AGI"),
        [EnemyParameters.Luck] = new("Luck", "LUK"),
    };

    private static readonly Dictionary<CharacterParameters, Tuple<string, string>> GameDataCharacterParameters = new()
    {
        [CharacterParameters.MapX] = new("Map X", "Map X"),
        [CharacterParameters.MapY] = new("Map Y", "Map Y"),
        [CharacterParameters.Direction] = new("Direction", "Direction"),
        [CharacterParameters.ScreenX] = new("Screen X", "Screen X"),
        [CharacterParameters.ScreenY] = new("Screen Y", "Screen Y"),
    };

    private static readonly Dictionary<OtherParameters, Tuple<string, string>> GameDataOtherParameters = new()
    {
        [OtherParameters.MapId] = new("Map ID", "Map ID"),
        [OtherParameters.PartyMembers] = new("Party Members", "Party Members"),
        [OtherParameters.Gold] = new("Gold", "Gold"),
        [OtherParameters.Steps] = new("Steps", "Steps"),
        [OtherParameters.PlayTime] = new("Play Time", "Play Time"),
        [OtherParameters.Timer] = new("Timer", "Timer"),
        [OtherParameters.SaveCount] = new("Save Count", "Save Count"),
        [OtherParameters.BattleCount] = new("Battle Count", "Battle Count"),
        [OtherParameters.WinCount] = new("Win Count", ""),
        [OtherParameters.EscapeCount] = new("Escape Count", ""),
    };
        
    public int StartVariable { get; set; }
    public int EndVariable { get; set; }
    public OperatorType Operator { get; set; }
    public OperandType Operand { get; set; }
    public object Value1 { get; set; }
    public object Value2 { get; set; }
    public object Value3 { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        StartVariable = parameterProvider.GetParameter<int>(0);
        EndVariable = parameterProvider.GetParameter<int>(1);
        Operator = (OperatorType)parameterProvider.GetParameter<int>(2);
        Operand = (OperandType)parameterProvider.GetParameter<int>(3);
        Value1 = parameterProvider.GetParameter<object>(4);
        Value2 = parameterProvider.GetParameter<object>(5);
        Value3 = parameterProvider.GetParameter<object>(6);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        parameterProvider.SetParameter(0, StartVariable);
        parameterProvider.SetParameter(1, EndVariable);
        parameterProvider.SetParameter(2, (int)Operator);
        parameterProvider.SetParameter(3, (int)Operand);
        parameterProvider.SetParameter(4, Value1);

        if (Operand == OperandType.GameData)
        {
            parameterProvider.SetParameter(5, Value2);
            switch ((GameDataType)Convert.ToInt32(Value1))
            {
                case GameDataType.Actor:
                case GameDataType.Enemy:
                case GameDataType.Character:
                    parameterProvider.SetParameter(6, Value3);
                    break;
            }
        }

        if (Operand == OperandType.Random)
            parameterProvider.SetParameter(5, Value2);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        string str;
        if (StartVariable != EndVariable)
            str = context.NameFormatter.FormatVariableSpan(StartVariable, EndVariable);
        else
        {
            if (context.Version == StorageVersion.MV)
            {
                str = context.NameFormatter.FormatVariable(StartVariable, new()
                {
                    BracketType = BracketType.None,
                    Separator = " ",
                    Prefix = "#",
                    SkipId = false,
                    SkipTitle = true,
                });
            }
            else
                str = context.NameFormatter.FormatVariable(StartVariable, new() { SkipTitle = true });
        }

        switch (Operator)
        {
            case OperatorType.Set:
                str += " = ";
                break;
            case OperatorType.Add:
                str += " += ";
                break;
            case OperatorType.Dec:
                str += " -= ";
                break;
            case OperatorType.Mul:
                str += " *=";
                break;
            case OperatorType.Div:
                str += " /= ";
                break;
            case OperatorType.Mod:
                str += " %= ";
                break;
            default:
                throw new Exception(
                    $"Unknown operator {Operator} for action #{Code} {GetCommandName(context.Version)}");
        }

        switch (Operand)
        {
            case OperandType.Constant:
                str += Convert.ToInt32(Value1);
                break;
            case OperandType.Variable:
                str += context.NameFormatter.FormatVariable(Convert.ToInt32(Value1), new()
                {
                    BracketType = BracketType.None
                });
                break;
            case OperandType.Random:
                if (context.Version == StorageVersion.MV)
                    str += $"Random {Convert.ToInt32(Value1)}..{Convert.ToInt32(Value2)}";
                else
                    str += $"Random No. ({Convert.ToInt32(Value1)}...{Convert.ToInt32(Value2)})";
                break;
            case OperandType.GameData:
                switch ((GameDataType)Convert.ToInt32(Value1))
                {
                    case GameDataType.ItemInInventory:
                        if (context.Version == StorageVersion.MV)
                            str += $"The number of {context.NameFormatter.FormatItem(Convert.ToInt32(Value2))}";
                        else
                            str += $"{context.NameFormatter.FormatItem(Convert.ToInt32(Value2))} in Inventory";
                        break;
                    case GameDataType.WeaponInInventory:
                        if (context.Version == StorageVersion.MV)
                            str += $"The number of {context.NameFormatter.FormatWeapon(Convert.ToInt32(Value2))}";
                        else
                            str += $"{context.NameFormatter.FormatWeapon(Convert.ToInt32(Value2))} in Inventory";
                        break;
                    case GameDataType.ArmorInInventory:
                        if (context.Version == StorageVersion.MV)
                            str += $"The number of {context.NameFormatter.FormatArmor(Convert.ToInt32(Value2))}";
                        else
                            str += $"{context.NameFormatter.FormatArmor(Convert.ToInt32(Value2))} in Inventory";
                        break;
                    case GameDataType.Actor:
                    {
                        var actorName = context.NameFormatter.FormatActor(Convert.ToInt32(Value2));
                        if (!GameDataActorParameters.TryGetValue((ActorParameters)Convert.ToInt32(Value3), out var paramName))
                            throw new Exception($"Unknown game data actor operand type {Convert.ToInt32(Value3)}");

                        var name = context.Version == StorageVersion.MV ? paramName.Item1 : paramName.Item2;
                        if (string.IsNullOrEmpty(name))
                            throw new Exception($"Unknown game data actor operand type {Convert.ToInt32(Value3)}");

                        if (context.Version == StorageVersion.MV)
                            str += $"{name} of {actorName}";
                        else
                            str += $"{actorName}'s {name}";
                        break;
                    }
                    case GameDataType.Enemy:
                    {
                        var enemyName = context.NameFormatter.FormatTroopMember(Convert.ToInt32(Value2));
                        if (!GameDataEnemyParameters.TryGetValue((EnemyParameters)Convert.ToInt32(Value3), out var paramName))
                            throw new Exception($"Unknown game data enemy operand type {Convert.ToInt32(Value3)}");

                        var name = context.Version == StorageVersion.MV ? paramName.Item1 : paramName.Item2;
                        if (string.IsNullOrEmpty(name))
                            throw new Exception($"Unknown game data enemy operand type {Convert.ToInt32(Value3)}");

                        if (context.Version == StorageVersion.MV)
                            str += $"{name} of {enemyName}";
                        else
                            str += $"{enemyName}'s {name}";
                        break;
                    }
                    case GameDataType.Character:
                    {
                        var eventName = context.NameFormatter.FormatEvent(Convert.ToInt32(Value2));

                        if (!GameDataCharacterParameters.TryGetValue((CharacterParameters)Convert.ToInt32(Value3), out var paramName))
                            throw new Exception($"Unknown game data character operand type {Convert.ToInt32(Value3)}");

                        var name = context.Version == StorageVersion.MV ? paramName.Item1 : paramName.Item2;
                        if (string.IsNullOrEmpty(name))
                            throw new Exception($"Unknown game data character operand type {Convert.ToInt32(Value3)}");

                        if (context.Version == StorageVersion.MV)
                            str += $"{name} of {eventName}";
                        else
                            str += $"{eventName}'s {name}";
                        break;
                    }
                    case GameDataType.Party:
                        if (context.Version == StorageVersion.MV)
                            str += $"Actor ID of the party member #{Convert.ToInt32(Value2) + 1}";
                        else
                            str += $"Member #{Convert.ToInt32(Value2) + 1}'s Actor ID";
                        break;
                    case GameDataType.Other:
                    {
                        if (!GameDataOtherParameters.TryGetValue((OtherParameters)Convert.ToInt32(Value2), out var otherName))
                            throw new Exception($"Unknown game data other operand type {Convert.ToInt32(Value2)}");

                        var name = context.Version == StorageVersion.MV ? otherName.Item1 : otherName.Item2;
                        if (string.IsNullOrEmpty(name))
                            throw new Exception($"Unknown game data other operand type {Convert.ToInt32(Value2)}");

                        str += name;
                        break;
                    }
                    default:
                        throw new Exception($"Unknown game data operand type {Convert.ToInt32(Value1)}");
                }

                break;
            case OperandType.Script:
                str += Convert.ToString(Value1);
                break;
            default:
                throw new Exception(
                    $"Unknown operator type #{Operand} for action #{Code} {GetCommandName(context.Version)}");
        }

        return str;
    }
}