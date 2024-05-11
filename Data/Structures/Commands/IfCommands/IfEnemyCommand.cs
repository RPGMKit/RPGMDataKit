using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfEnemyCommand : IfCommand
    {
        public enum CheckType
        {
            Appeared = 0,
            State = 1,
        }

        public int GetTroopMemberId()
        {
            return GetParameter<int>(1);
        }

        public CheckType GetCheckType()
        {
            return (CheckType)GetParameter<int>(2);
        }

        public T? GetCheckParameter<T>()
        {
            return GetParameter<T>(3);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatTroopMember(GetTroopMemberId());

            switch (GetCheckType())
            {
                case CheckType.Appeared:
                    if (context.Version == StorageVersion.MV)
                        str += " is appeared";
                    str += " is Appeared";
                    break;
                case CheckType.State:
                    if (context.Version == StorageVersion.MV)
                        str += $" is affected by {context.NameFormatter.FormatState(GetCheckParameter<int>())}";
                    else
                        str += $" is {context.NameFormatter.FormatState(GetCheckParameter<int>())} Inflicted";
                    break;
                default:
                    throw new Exception(
                        $"Unknown check type '{GetCheckType()}' for '{GetCommandName(context.Version)}' enemy command");
            }

            return str;
        }
    }
}
