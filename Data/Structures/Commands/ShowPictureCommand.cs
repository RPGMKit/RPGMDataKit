using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.ParameterProviders;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands;

public class ShowPictureCommand : NamedCommand
{
    public override int Code => CommandCodes.ShowPicture;
    public override string GetCommandName(StorageVersion version) => "Show picture";
    
    public enum ReferenceType
    {
        UpperLeft = 0,
        Center = 1,
    }
        
    public enum DesignationType
    {
        Direct = 0,
        ByVariable = 1,
    }
        
    public int Layer { get; set; }
    public string Picture { get; set; }
    public ReferenceType Reference { get; set; }
    public DesignationType Designation { get; set; }
    public int Param1 { get; set; }
    public int Param2 { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Transparency { get; set; }
    public BlendMode BlendMode { get; set; }

    public override void Unpack(IParameterProvider parameterProvider, StorageVersion version)
    {
        Layer = parameterProvider.GetParameter<int>(0);
        Picture = parameterProvider.GetParameter<string>(1);
        Reference = parameterProvider.GetParameter<ReferenceType>(2);
        Designation = parameterProvider.GetParameter<DesignationType>(3);
        Param1 = parameterProvider.GetParameter<int>(4);
        Param2 = parameterProvider.GetParameter<int>(5);
        Width = parameterProvider.GetParameter<int>(6);
        Height = parameterProvider.GetParameter<int>(7);    
        Transparency = parameterProvider.GetParameter<int>(8);
        BlendMode = parameterProvider.GetParameter<BlendMode>(9);
    }

    public override void Pack(IParameterProvider parameterProvider, StorageVersion version)
    {
        if (BlendMode == BlendMode.Screen && version != StorageVersion.MV)
            throw new Exception($"Unsupported blend mode {BlendMode}");

        parameterProvider.SetParameter(0, Layer);
        parameterProvider.SetParameter(1, Picture);
        parameterProvider.SetParameter(2, Reference);
        parameterProvider.SetParameter(3, Designation);
        parameterProvider.SetParameter(4, Param1);
        parameterProvider.SetParameter(5, Param2);
        parameterProvider.SetParameter(6, Width);
        parameterProvider.SetParameter(7, Height);
        parameterProvider.SetParameter(8, Transparency);
        parameterProvider.SetParameter(9, BlendMode);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var pictureName = Picture;
        if (string.IsNullOrWhiteSpace(pictureName) && context.Version == StorageVersion.MV)
            pictureName = "None";

        string str;
        if (context.Version == StorageVersion.MV)
            str = $"#{Layer}, {pictureName}, ";
        else
            str = $"{Layer}, '{pictureName}', ";

        str += Reference switch
        {
            ReferenceType.UpperLeft => "Upper left",
            ReferenceType.Center => "Center",
            _ => throw new Exception($"Unknown show picture reference type '{Reference}'")
        };

        switch (Designation)
        {
            case DesignationType.Direct:
                str += $" ({Param1},{Param2})";
                break;
            case DesignationType.ByVariable:
                string var1, var2;
                if (context.Version == StorageVersion.MV)
                {
                    var1 = context.NameFormatter.FormatVariable(Param1);
                    var2 = context.NameFormatter.FormatVariable(Param2);

                    str += $" ({var1},{var2})";
                }
                else
                {
                    var1 = context.NameFormatter.FormatVariable(Param1, new()
                    {
                        SkipTitle = true,
                        SkipName = true,
                    });
                    var2 = context.NameFormatter.FormatVariable(Param2, new()
                    {
                        SkipTitle = true,
                        SkipName = true,
                    });

                    str += $" (Variable {var1}{var2})";
                }

                break;
            default:
                throw new Exception($"Unknown show picture place by type '{Designation}'");
        }

        str += $", ({Width}%,{Height}%), {Transparency}, ";
        switch (BlendMode)
        {
            case BlendMode.Normal:
                str += "Normal";
                break;
            case BlendMode.Add:
                str += context.Version == StorageVersion.MV ? "Additive" : "Add";
                break;
            case BlendMode.Sub:
            //case BlendMode.Multiply:
                str += context.Version == StorageVersion.MV
                    ? "Multiply"
                    : "Sub";
                break;
            case BlendMode.Screen:
                if (context.Version == StorageVersion.MV)
                    str += "Screen";
                else
                    throw new Exception($"Unknown show picture blend mode '{BlendMode}'");
                break;
            default:
                throw new Exception($"Unknown show picture blend mode '{BlendMode}'");
        }

        return str;
    }
}