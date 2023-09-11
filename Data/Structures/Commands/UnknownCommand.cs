using Newtonsoft.Json;
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

        public override string Name => $"Unknown ({Code})";

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return JsonConvert.SerializeObject(Parameters);
        }
    }
}
