using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ControlVariableCommand : NamedCommand
    {
        protected const int OPERATOR_SET = 0;
        protected const int OPERATOR_ADD = 1;
        protected const int OPERATOR_DEC = 2;
        protected const int OPERATOR_MUL = 3;
        protected const int OPERATOR_DIV = 4;
        protected const int OPERATOR_MOD = 5;

        protected const int OPERAND_CONSTANT = 0;
        protected const int OPERAND_VARIABLE = 1;
        protected const int OPERAND_RANDOM = 2;
        protected const int OPERAND_GAME_DATA = 3;
        protected const int OPERAND_SCRIPT = 4;

        protected const int GameDataItemInInventory = 0;
        protected const int GameDataWeaponInInventory = 1;
        protected const int GameDataArmorInInventory = 2;
        protected const int GameDataActor = 3;
        protected const int GameDataEnemy = 4;
        protected const int GameDataCharacter = 5;
        protected const int GameDataParty = 6;
        protected const int GameDataOther = 7;

        private static Dictionary<int, Tuple<string, string>> GameDataActorParameters = new()
        {
            [0] = new("Level", "Level"),
            [1] = new("EXP", "EXP"),
            [2] = new("HP", "HP"),
            [3] = new("MP", "MP"),
            [4] = new("Max HP", "MHP"),
            [5] = new("Max MP", "MMP"),
            [6] = new("Attack", "ATK"),
            [7] = new("Defense", "DEF"),
            [8] = new("M.Attack", "MAT"),
            [9] = new("M.Defense", "MDF"),
            [10] = new("Agility", "AGI"),
            [11] = new("Luck", "LUK"),
        };

        private static Dictionary<int, Tuple<string, string>> GameDataEnemyParameters = new()
        {
            [0] = new("HP", "HP"),
            [1] = new("MP", "MP"),
            [2] = new("Max HP", "MHP"),
            [3] = new("Max MP", "MMP"),
            [4] = new("Attack", "ATK"),
            [5] = new("Defense", "DEF"),
            [6] = new("M.Attack", "MAT"),
            [7] = new("M.Defense", "MDF"),
            [8] = new("Agility", "AGI"),
            [9] = new("Luck", "LUK"),
        };

        private static Dictionary<int, Tuple<string, string>> GameDataCharacterParameters = new()
        {
            [0] = new("Map X", "Map X"),
            [1] = new("Map Y", "Map Y"),
            [2] = new("Direction", "Direction"),
            [3] = new("Screen X", "Screen X"),
            [4] = new("Screen Y", "Screen Y"),
        };

        private static Dictionary<int, Tuple<string, string>> GameDataOtherParameters = new()
        {
            [0] = new("Map ID", "Map ID"),
            [1] = new("Party Members", "Party Members"),
            [2] = new("Gold", "Gold"),
            [3] = new("Steps", "Steps"),
            [4] = new("Play Time", "Play Time"),
            [5] = new("Timer", "Timer"),
            [6] = new("Save Count", "Save Count"),
            [7] = new("Battle Count", "Battle Count"),
            [8] = new("Win Count", ""),
            [9] = new("Escape Count", ""),
        };

        public int GetStartVariable()
        {
            return GetParameter<int>(0);
        }

        public int GetEndVariable()
        {
            return GetParameter<int>(1);
        }

        public int GetOperator()
        {
            return GetParameter<int>(2);
        }

        public int GetOperandType()
        {
            return GetParameter<int>(3);
        }

        public T GetOperand1<T>()
        {
            return GetParameter<T>(4);
        }

        public T GetOperand2<T>()
        {
            return GetParameter<T>(5);
        }

        public T GetOperand3<T>()
        {
            return GetParameter<T>(6);
        }

        public override int Code => CommandCodes.ControlVariables;
        public override string GetCommandName(StorageVersion version) => "Control Variables";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            string str;
            if (GetStartVariable() != GetEndVariable())
                str = context.NameFormatter.FormatVariableSpan(GetStartVariable(), GetEndVariable());
            else
            {
                if (context.Version == StorageVersion.MV)
                {
                    str = context.NameFormatter.FormatVariable(GetStartVariable(), new()
                    {
                        Separator = " ",
                        Prefix = "#",
                        SkipId = false,
                        SkipTitle = true,
                    });
                }
                else
                    str = context.NameFormatter.FormatVariable(GetStartVariable(), new() { SkipTitle = true });
            }

            switch (GetOperator())
            {
                case OPERATOR_SET:
                    str += " = ";
                    break;
                case OPERATOR_ADD:
                    str += " += ";
                    break;
                case OPERATOR_DEC:
                    str += " -= ";
                    break;
                case OPERATOR_MUL:
                    str += " *=";
                    break;
                case OPERATOR_DIV:
                    str += " /= ";
                    break;
                case OPERATOR_MOD:
                    str += " %= ";
                    break;
                default:
                    throw new Exception(
                        $"Unknown operator type #{GetOperator()} for action #{Code} {GetCommandName(context.Version)}");
            }

            switch (GetOperandType())
            {
                case OPERAND_CONSTANT:
                    str += GetOperand1<int>();
                    break;
                case OPERAND_VARIABLE:
                    str += context.NameFormatter.FormatVariable(GetOperand1<int>());
                    break;
                case OPERAND_RANDOM:
                    if (context.Version == StorageVersion.MV)
                        str += $"Random {GetOperand1<int>()}..{GetOperand2<int>()}";
                    else
                        str += $"Random No. ({GetOperand1<int>()}...{GetOperand2<int>()})";
                    break;
                case OPERAND_GAME_DATA:
                    switch (GetOperand1<int>())
                    {
                        case GameDataItemInInventory:
                            if (context.Version == StorageVersion.MV)
                                str += $"The number of {context.NameFormatter.FormatItem(GetOperand2<int>())}";
                            else
                                str += $"{context.NameFormatter.FormatItem(GetOperand2<int>())} in Inventory";
                            break;
                        case GameDataWeaponInInventory:
                            if (context.Version == StorageVersion.MV)
                                str += $"The number of {context.NameFormatter.FormatWeapon(GetOperand2<int>())}";
                            else
                                str += $"{context.NameFormatter.FormatWeapon(GetOperand2<int>())} in Inventory";
                            break;
                        case GameDataArmorInInventory:
                            if (context.Version == StorageVersion.MV)
                                str += $"The number of {context.NameFormatter.FormatArmor(GetOperand2<int>())}";
                            else
                                str += $"{context.NameFormatter.FormatArmor(GetOperand2<int>())} in Inventory";
                            break;
                        case GameDataActor:
                        {
                            var actorName = context.NameFormatter.FormatActor(GetOperand2<int>());
                            if (!GameDataActorParameters.TryGetValue(GetOperand3<int>(), out var paramName))
                                throw new Exception($"Unknown game data actor operand type {GetOperand3<int>()}");

                            var name = context.Version == StorageVersion.MV ? paramName.Item1 : paramName.Item2;
                            if (string.IsNullOrEmpty(name))
                                throw new Exception($"Unknown game data actor operand type {GetOperand3<int>()}");

                            if (context.Version == StorageVersion.MV)
                                str += $"{name} of {actorName}";
                            else
                                str += $"{actorName}'s {name}";
                            break;
                        }
                        case GameDataEnemy:
                        {
                            var enemyName = context.NameFormatter.FormatTroopMember(GetOperand2<int>());
                            if (!GameDataEnemyParameters.TryGetValue(GetOperand3<int>(), out var paramName))
                                throw new Exception($"Unknown game data enemy operand type {GetOperand3<int>()}");

                            var name = context.Version == StorageVersion.MV ? paramName.Item1 : paramName.Item2;
                            if (string.IsNullOrEmpty(name))
                                throw new Exception($"Unknown game data enemy operand type {GetOperand3<int>()}");

                            if (context.Version == StorageVersion.MV)
                                str += $"{name} of {enemyName}";
                            else
                                str += $"{enemyName}'s {name}";
                            break;
                        }
                        case GameDataCharacter:
                        {
                            var eventName = context.NameFormatter.FormatEvent(GetOperand2<int>());
                            
                            if (!GameDataCharacterParameters.TryGetValue(GetOperand3<int>(), out var paramName))
                                throw new Exception($"Unknown game data character operand type {GetOperand3<int>()}");

                            var name = context.Version == StorageVersion.MV ? paramName.Item1 : paramName.Item2;
                            if (string.IsNullOrEmpty(name))
                                throw new Exception($"Unknown game data character operand type {GetOperand3<int>()}");

                            if (context.Version == StorageVersion.MV)
                                str += $"{name} of {eventName}";
                            else
                                str += $"{eventName}'s {name}";
                            break;
                        }
                        case GameDataParty:
                            if (context.Version == StorageVersion.MV)
                                str += $"Actor ID of the party member #{GetOperand2<int>() + 1}";
                            else
                                str += $"Member #{GetOperand2<int>() + 1}'s Actor ID";
                            break;
                        case GameDataOther:
                        {
                            if (!GameDataOtherParameters.TryGetValue(GetOperand2<int>(), out var otherName))
                                throw new Exception($"Unknown game data other operand type {GetOperand2<int>()}");

                            var name = context.Version == StorageVersion.MV ? otherName.Item1 : otherName.Item2;
                            if (string.IsNullOrEmpty(name))
                                throw new Exception($"Unknown game data other operand type {GetOperand2<int>()}");

                            str += name;
                            break;
                        }
                        default:
                            throw new Exception($"Unknown game data operand type {GetOperand1<int>()}");
                    }

                    break;
                case OPERAND_SCRIPT:
                    str += GetOperand1<string>();
                    break;
                default:
                    throw new Exception(
                        $"Unknown operator type #{GetOperandType()} for action #{Code} {GetCommandName(context.Version)}");
            }

            return str;
        }
    }
}
