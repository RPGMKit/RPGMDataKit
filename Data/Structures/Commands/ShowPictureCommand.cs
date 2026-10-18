using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

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
        
    [EditorDefault(1)]
    public int Layer
    {
        get => Get<int>(0);
        set => Set(0, value);
    }

    [EditorDefault("")]
    public string Picture
    {
        get => Get<string>(1);
        set => Set(1, value);
    }

    public ReferenceType Reference
    {
        get => Get<ReferenceType>(2);
        set => Set(2, value);
    }

    public DesignationType Designation
    {
        get => Get<DesignationType>(3);
        set => Set(3, value);
    }

    public int Param1
    {
        get => Get<int>(4);
        set => Set(4, value);
    }

    public int Param2
    {
        get => Get<int>(5);
        set => Set(5, value);
    }

    [EditorDefault(100)]
    public int Width
    {
        get => Get<int>(6);
        set => Set(6, value);
    }

    [EditorDefault(100)]
    public int Height
    {
        get => Get<int>(7);
        set => Set(7, value);
    }

    [EditorDefault(255)]
    public int Transparency
    {
        get => Get<int>(8);
        set => Set(8, value);
    }

    public BlendMode BlendMode
    {
        get => Get<BlendMode>(9);
        set
        {
            if (value == BlendMode.Screen)
                RequireVersion(StorageVersion.MV, nameof(BlendMode) + " " + value);

            Set(9, value);
        }
    }

    // a command made in code does not know its maker yet, so the blend mode that only MV has is refused when it is put into a list of VX Ace
    protected override void OnBinding(StorageVersion version)
    {
        if (version != StorageVersion.MV && Get<BlendMode>(9) == BlendMode.Screen)
            throw new DoesNotExistInVersionException($"{BlendMode.Screen} exists in MV only");
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