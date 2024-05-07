using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfVehicleCommand : IfCommand
    {
        public string GetVehicle()
        {
            return GetParameter<string>(1);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return $"#'{GetVehicle()}' is driven";
        }
    }
}
