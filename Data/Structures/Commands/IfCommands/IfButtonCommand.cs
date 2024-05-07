using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.IfCommands
{
    public class IfButtonCommand : IfCommand
    {
        private static Dictionary<int, string> buttonIdToName = new()
        {
            [2] = "Down",
            [4] = "Left",
            [6] = "Right",
            [8] = "Up",
            [11] = "A",
            [12] = "B",
            [13] = "C",
            [14] = "X",
            [15] = "Y",
            [16] = "Z",
            [17] = "L",
            [18] = "r",
        };

        public string GetButton(StorageVersion version)
        {
            if (version == StorageVersion.VXAce)
            {
                var index = GetParameter<int>(1);
                return buttonIdToName.ContainsKey(index) ? buttonIdToName[index] : $"#{index}";
            }

            return GetParameter<string>(1);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            return context.Version == StorageVersion.VXAce
                ? $"The {GetButton(context.Version)} Button is Being Pressed"
                : $"Button [{GetButton(context.Version)}] is pressed down";
        }
    }
}
