namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfButtonCommand : IfCommand
    {
        public string GetButton()
        {
            return GetParameter<string>(1);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"Button [{GetButton()}] is pressed";
        }
    }
}
