using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeEquipmentCommand : NamedCommand
    {
        public override int Code => CommandCodes.ChangeEquipment;
        public override string Name => "Change Equipment";

        public int GetActor()
        {
            return (int)GetParameter(0);
        }

        public int GetEquipmentType()
        {
            return (int)GetParameter(1);
        }
        
        public int GetEquipmentItem()
        {
            return (int)GetParameter(2);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"#{GetActor()}, #{GetEquipmentType()} = #{GetEquipmentItem()}";

            return str;
        }
    }
}