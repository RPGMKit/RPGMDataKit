using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfScriptCommand : IfCommand
    {
        public string GetScenario()
        {
            return GetParameter<string>(1);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"Script: '{GetScenario()}'";
        }
    }
}
