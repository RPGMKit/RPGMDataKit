using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using MVTone = RPGMakerLibCoreV2.Data.MV.Classes.Tone;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ChangeWindowColorCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeWindowColor;
    public override string GetCommandName(StorageVersion version) => "Change Window Color";

    public ITone GetColor(StorageVersion version)
    {
        if (version == StorageVersion.MV)
            return MVTone.FromArray(GetParameter<List<int>>(0));
                
        return GetParameter<ITone>(0);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var color = GetColor(context.Version);

        return $"({color.Red},{color.Green},{color.Blue})";
    }
}