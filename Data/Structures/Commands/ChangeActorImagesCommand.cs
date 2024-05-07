using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ChangeActorImagesCommand : NamedCommand
    {
        public override int Code => CommandCodes.ChangeActorImages;

        public override string GetCommandName(StorageVersion version)
        {
            if (version == StorageVersion.MV)
                return "Change Actor Images";
            else
                return "Change Actor Images";
        }

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

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatActor(GetActor());

            var format = context.Version != StorageVersion.MV ? ", {0}({1})" : ", '{0}', {1}";
            var none = context.Version == StorageVersion.MV ? ", None" : ", (None)";
            
            var faceImage = GetFaceImage();
            if (!string.IsNullOrEmpty(faceImage))
                str += string.Format(format, faceImage, GetFaceImageOffset());
            else
                str += none;

            var characterImage = GetCharacterImage();
            if (!string.IsNullOrEmpty(characterImage))
                str += string.Format(format, characterImage, GetCharacterImageOffset());
            else
                str += none;

            if (context.Version == StorageVersion.MV)
            {
                var swBattler = GetSwBattler();
                if (!string.IsNullOrEmpty(swBattler))
                    str += $", {swBattler}";
                else
                    str += none;
            }

            return str;
        }
    }
}