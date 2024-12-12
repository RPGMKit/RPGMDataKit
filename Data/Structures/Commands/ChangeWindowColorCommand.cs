using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;
using RPGMakerLibCoreV2.Data.VXAce.Classes.Custom;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

using VXTone = Tone;
using MVTone = RPGMakerLibCoreV2.Data.MV.Classes.Tone;

public class ChangeWindowColorCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeWindowColor;
    public override string GetCommandName(StorageVersion version) => "Change Window Color";

    public ITone Tone { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        if (version == StorageVersion.MV)
            Tone = MVTone.FromArray(parameterProvider.GetParameter<List<int>>(0));
        else
            Tone = parameterProvider.GetParameter<ITone>(0);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        if (version == StorageVersion.MV)
            parameterProvider.SetParameter(0, new List<int>() { Tone.Red, Tone.Green, Tone.Blue, Tone.Gray });
        else
            parameterProvider.SetParameter(0,
                new VXTone() { Red = Tone.Red, Green = Tone.Green, Blue = Tone.Blue, Gray = Tone.Gray });
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        return $"({Tone.Red},{Tone.Green},{Tone.Blue})";
    }
}