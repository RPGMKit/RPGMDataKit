using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class RecoverAllCommand : NamedCommand
    {
        public enum SourceType
        {
            Actor = 0,
            Variable = 1,
        }

        public SourceType GetSourceType()
        {
            return (SourceType)GetParameter<int>(0);
        }

        public int GetSource()
        {
            return GetParameter<int>(1);
        }

        public override int Code => CommandCodes.RecoverAll;
        public override string GetCommandName(StorageVersion version) => "Recover All";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var source = GetSource();
            var str = "";
            switch (GetSourceType())
            {
                case SourceType.Actor:
                    str = context.NameFormatter.FormatActor(source);
                    break;
                case SourceType.Variable:
                    str = context.NameFormatter.FormatVariable(source);
                    break;
                default:
                    throw new Exception($"Unknown recover all source type '{GetSourceType()}'");
            }

            return str;
        }
    }
}
