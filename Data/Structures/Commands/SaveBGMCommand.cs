using System;
using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SaveBGMCommand : NamedCommand
    {
        public override int Code => CommandCodes.SaveBgm;
        public override string Name => "Save BGM";
    }
}
