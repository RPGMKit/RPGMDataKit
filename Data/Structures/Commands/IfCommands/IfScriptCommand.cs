namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfScriptCommand : IfCommand
    {
        public string GetScenario()
        {
            return (string)GetParameter(1);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"Script: '{GetScenario()}'";
        }
    }
}
