using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class UnknownCommand : NamedCommand
    {
        private readonly int _code;

        public UnknownCommand(int code)
        {
            _code = code;
        }

        public override int Code => _code;

        public override string GetCommandName(StorageVersion version) => $"Unknown ({Code})";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            throw new Exception("not implemented");
            // return JsonConvert.SerializeObject(Parameters);
        }
    }
}
