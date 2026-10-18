using RPGMDataKit.Data.Structures.Attributes;
using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class MovePictureCommand : NamedCommand
{
    public override int Code => CommandCodes.MovePicture;
    public override string GetCommandName(StorageVersion version) => "Move picture";
        
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
        
    // parameter 1 is always 0

    [EditorDefault(1)]
    public int Layer
    {
        get => Get<int>(0);
        set => Set(0, value);
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

    [EditorDefault(60)]
    public int Frames
    {
        get => Get<int>(10);
        set => Set(10, value);
    }

    [EditorDefault(true)]
    public bool WaitCompletion
    {
        get => Get<bool>(11);
        set => Set(11, value);
    }

    // a command made in code does not know its maker yet, so the blend mode that only MV has is refused when it is put into a list of VX Ace
    protected override void OnBinding(StorageVersion version)
    {
        if (version != StorageVersion.MV && Get<BlendMode>(9) == BlendMode.Screen)
            throw new DoesNotExistInVersionException($"{BlendMode.Screen} exists in MV only");
    }

    // the second parameter is not used: 0 in MV (it cannot be blank there), nil in VX Ace
    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        Set(1, version == StorageVersion.MV ? 0 : (int?)null);
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = $"{Layer}";
        if (context.Version == StorageVersion.MV)
            str = "#" + str;

        str += Reference switch
        {
            ReferenceType.UpperLeft => ", Upper left",
            ReferenceType.Center => ", Center",
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
        str += BlendMode switch
        {
            BlendMode.Normal => "Normal",
            BlendMode.Add => context.Version == StorageVersion.MV ? "Additive" : "Add",
            BlendMode.Sub => context.Version == StorageVersion.MV ? "Multiply" : "Sub",
            BlendMode.Screen => context.Version == StorageVersion.MV ? "Screen" : $"Unknown ({BlendMode})",
            _ => throw new Exception($"Unknown show picture blend mode '{BlendMode}'")
        };

        str += context.Version == StorageVersion.MV ? $", {Frames} frames" : $", @{Frames}";
        if (WaitCompletion)
            str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

        return str;
    }
}