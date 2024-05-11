using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeEnemyStateCommand : NamedCommand
    {
        public enum OperatorType
        {
            Add = 0,
            Remove = 1,
        }

        public int GetEnemy()
        {
            return GetParameter<int>(0);
        }

        public OperatorType GetOperator()
        {
            return (OperatorType)GetParameter<int>(1);
        }

        public int GetState()
        {
            return GetParameter<int>(2);
        }

        public override int Code => CommandCodes.ChangeEnemyState;
        public override string GetCommandName(StorageVersion version) => "Change Enemy State";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatTroopMember(GetEnemy());

            switch (GetOperator())
            {
                case OperatorType.Add:
                    str += ", + ";
                    break;
                case OperatorType.Remove:
                    str += ", - ";
                    break;
                default:
                    throw new Exception($"Unknown change enemy state operator type '{GetOperator()}'");
            }

            str += context.NameFormatter.FormatState(GetState());

            return str;
        }
    }
}
