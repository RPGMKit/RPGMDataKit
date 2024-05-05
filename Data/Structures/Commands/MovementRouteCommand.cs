using Newtonsoft.Json.Linq;
using RPGMakerLibCoreV2.Data.Structures.Commands.MovementCommands;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.MV.Classes;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories.MovementCommands;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public abstract class MovementRouteCommand : NamedCommand
    {
        public MovementRouteCommand()
        {
            IsSecondary = true;
        }

        public override int Code => CommandCodes.MovementRouteCommand;
        public override string Name => "                  ";

        public abstract NamedCommand Factory();

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var moveCommand = Factory();

            return moveCommand.AsString(storage, new DumpOptions() {AppendCommandCode = options.AppendCommandCode, WithIndent = false, EmphasiseStrings = options.EmphasiseStrings});
        }
    }
}
