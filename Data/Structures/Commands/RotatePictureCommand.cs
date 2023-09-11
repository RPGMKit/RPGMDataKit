using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class RotatePictureCommand : NamedCommand
    {
        public override int Code => CommandCodes.RotatePicture;
        public override string Name => "Rotate Picture";

        public int GetLayer()
        {
            return (int)GetParameter(0);
        }

        public int GetSpeed()
        {
            return (int)GetParameter(1);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"#{GetLayer()}, {GetSpeed()}";
        }
    }
}
