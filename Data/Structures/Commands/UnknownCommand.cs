using Newtonsoft.Json;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

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

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            throw new Exception("not implemented");
            // return JsonConvert.SerializeObject(Parameters);
        }
    }
}
