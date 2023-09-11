namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfVehicleCommand : IfCommand
    {
        public string GetVehicle()
        {
            return (string)GetParameter(1);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"#'{GetVehicle()}' is driven";
        }
    }
}
