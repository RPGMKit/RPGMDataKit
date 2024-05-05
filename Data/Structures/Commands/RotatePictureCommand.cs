using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class RotatePictureCommand : NamedCommand
    {
        public override int Code => CommandCodes.RotatePicture;
        public override string Name => "Rotate Picture";

        public int GetLayer()
        {
            return GetParameter<int>(0);
        }

        public int GetSpeed()
        {
            return GetParameter<int>(1);
        }

        public override string GetParametersDescription(Storage storage, DumpOptions options)
        {
            return $"#{GetLayer()}, {GetSpeed()}";
        }
    }
}
