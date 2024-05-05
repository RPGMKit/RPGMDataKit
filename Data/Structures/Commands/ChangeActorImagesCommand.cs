using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeActorImagesCommand : NamedCommand
    {
        public override int Code => CommandCodes.ChangeActorImages;
        public override string Name => "Change Actor Images";

        public int GetActor()
        {
            return GetParameter<int>(0);
        }

        public string GetFaceImage()
        {
            return GetParameter<string>(1);
        }

        public int GetFaceImageOffset()
        {
            return GetParameter<int>(2);
        }

        public string GetCharacterImage()
        {
            return GetParameter<string>(3);
        }

        public int GetCharacterImageOffset()
        {
            return GetParameter<int>(4);
        }

        public string GetSwBattler()
        {
            var param = GetParameter<string>(5);

            return param ?? "";
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            var str = $"Actor #{GetActor()}";

            var faceImage = GetFaceImage();
            if (!string.IsNullOrEmpty(faceImage))
                str += $", {faceImage}({GetFaceImageOffset()})";
            else
                str += ", None";

            var characterImage = GetCharacterImage();
            if (!string.IsNullOrEmpty(characterImage))
                str += $", {characterImage}({GetCharacterImageOffset()})";
            else
                str += ", None";

            var swBattler = GetSwBattler();
            if (!string.IsNullOrEmpty(swBattler))
                str += $", {swBattler}";
            else
                str += ", None";

            return str;
        }
    }
}