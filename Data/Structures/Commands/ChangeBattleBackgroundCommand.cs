using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeBattleBackground : NamedCommand
    {
        public override int Code => CommandCodes.ChangeBattleBackground;
        public override string GetCommandName(StorageVersion version) => "Change Battle Back";

        public string GetImage1()
        {
            return GetParameter<string>(0);
        }
        
        public string GetImage2()
        {
            return GetParameter<string>(1);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var image1 = GetImage1();
            var image2 = GetImage2();

            if (!string.IsNullOrEmpty(image2))
                return $"{image1} & {image2}";

            return image1;
        }
    }
}
