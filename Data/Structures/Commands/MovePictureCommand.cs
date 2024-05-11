using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class MovePictureCommand : NamedCommand
    {
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
        
        public int GetLayer()
        {
            return GetParameter<int>(0);
        }

        public ReferenceType GetReference()
        {
            return (ReferenceType)GetParameter<int>(2);
        }

        public DesignationType GetDesignation()
        {
            return (DesignationType)GetParameter<int>(3);
        }

        public int GetParam1()
        {
            return GetParameter<int>(4);
        }

        public int GetParam2()
        {
            return GetParameter<int>(5);
        }

        public int GetWidth()
        {
            return GetParameter<int>(6);
        }

        public int GetHeight()
        {
            return GetParameter<int>(7);
        }

        public int GetTransparency()
        {
            return GetParameter<int>(8);
        }

        public BlendMode GetBlendMode()
        {
            return (BlendMode)GetParameter<int>(9);
        }

        public int GetFrames()
        {
            return GetParameter<int>(10);
        }

        public bool GetWaitCompletion()
        {
            return GetParameter<bool>(11);
        }

        public override int Code => CommandCodes.MovePicture;
        public override string GetCommandName(StorageVersion version) => "Move picture";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = $"{GetLayer()}";
            if (context.Version == StorageVersion.MV)
                str = "#" + str;
            
            switch (GetReference())
            {
                case ReferenceType.UpperLeft:
                    str += ", Upper left";
                    break;
                case ReferenceType.Center:
                    str += ", Center";
                    break;
                default:
                    throw new Exception($"Unknown show picture reference type '{GetReference()}'");
            }

            switch (GetDesignation())
            {
                case DesignationType.Direct:
                    str += $" ({GetParam1()},{GetParam2()})";
                    break;
                case DesignationType.ByVariable:
                    string var1, var2;
                    if (context.Version == StorageVersion.MV)
                    {
                        var1 = context.NameFormatter.FormatVariable(GetParam1());
                        var2 = context.NameFormatter.FormatVariable(GetParam2());

                        str += $" ({var1},{var2})";
                    }
                    else
                    {
                        var1 = context.NameFormatter.FormatVariable(GetParam1(), new()
                        {
                            SkipTitle = true,
                            SkipName = true,
                        });
                        var2 = context.NameFormatter.FormatVariable(GetParam2(), new()
                        {
                            SkipTitle = true,
                            SkipName = true,
                        });

                        str += $" (Variable {var1}{var2})";
                    }
                    break;
                default:
                    throw new Exception($"Unknown show picture place by type '{GetDesignation()}'");
            }

            str += $", ({GetWidth()}%,{GetHeight()}%), {GetTransparency()}, ";
            switch (GetBlendMode())
            {
                case BlendMode.Normal:
                    str += "Normal";
                    break;
                case BlendMode.Add:
                    str += context.Version == StorageVersion.MV ? "Additive" : "Add";
                    break;
                case BlendMode.Sub:
                    str += context.Version == StorageVersion.MV ? throw new Exception($"Unknown show picture blend mode '{GetBlendMode()}'") : "Sub";
                    break;
                default:
                    throw new Exception($"Unknown show picture blend mode '{GetBlendMode()}'");
            }

            str += context.Version == StorageVersion.MV ? $", {GetFrames()} frames" : $", @{GetFrames()}";
            if (GetWaitCompletion())
                str += context.Version == StorageVersion.MV ? " (Wait)" : ", Wait";

            return str;
        }
    }
}
