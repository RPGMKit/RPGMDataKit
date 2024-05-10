using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfEnemyCommand : IfCommand
    {
        protected const int CHECK_APPEARED = 0;
        protected const int CHECK_STATE = 1;

        public int GetTroopMemberId()
        {
            return GetParameter<int>(1);
        }

        public int GetCheckType()
        {
            return GetParameter<int>(2);
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
                case CHECK_APPEARED:
                    if (context.Version == StorageVersion.MV)
                        str += " is appeared";
                    str += " is Appeared";
                    break;
                case CHECK_STATE:
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
