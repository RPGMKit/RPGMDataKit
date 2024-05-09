using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShowPictureCommand : NamedCommand
    {
        protected const int REFERENCE_UPPERLEFT = 0;
        protected const int REFERENCE_CENTER = 1;
        protected const int PLACE_DIRECT = 0;
        protected const int PLACE_BY_VARIABLE = 1;
        protected const int BLEND_NORMAL = 0;
        protected const int BLEND_ADDITIVE = 1;
        protected const int BLEND_SUB = 2;

        public int GetLayer()
        {
            return GetParameter<int>(0);
        }

        public string GetPicture()
        {
            return GetParameter<string>(1);
        }

        public int GetReference()
        {
            return GetParameter<int>(2);
        }

        public int GetPlaceType()
        {
            return GetParameter<int>(3);
        }

        public int GetParam1()
        {
            return GetParameter<int>(4);
        }

        public int GetParam2()
        {
            return GetParameter<int>(5);
        }

        public void SetY(int value)
        {
            SetParameter(5, value);
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

        public int GetBlendMode()
        {
            return GetParameter<int>(9);
        }

        public void SetLayer(int value)
        {
            SetParameter(0, value);
        }

        public void SetPicture(string value)
        {
            SetParameter(1, value);
        }

        public override int Code => CommandCodes.ShowPicture;
        public override string GetCommandName(StorageVersion version) => "Show picture";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var pictureName = GetPicture();
            if (string.IsNullOrWhiteSpace(pictureName) && context.Version == StorageVersion.MV)
                pictureName = "None";

            string str;
            if (context.Version == StorageVersion.MV)
                str = $"#{GetLayer()}, {pictureName}, ";
            else
                str = $"{GetLayer()}, '{pictureName}', ";

            switch (GetReference())
            {
                case REFERENCE_UPPERLEFT:
                    str += "Upper left";
                    break;
                case REFERENCE_CENTER:
                    str += "Center";
                    break;
                default:
                    throw new Exception($"Unknown show picture reference type '{GetReference()}'");
            }

            switch (GetPlaceType())
            {
                case PLACE_DIRECT:
                    str += $" ({GetParam1()},{GetParam2()})";
                    break;
                case PLACE_BY_VARIABLE:
                    string var1, var2;
                    if (context.Version == StorageVersion.MV)
                    {
                        var1 = context.NameFormatter.FormatVariable(GetParam1(),
                            new() { BracketType = BracketType.Figure });
                        var2 = context.NameFormatter.FormatVariable(GetParam2(),
                            new() { BracketType = BracketType.Figure });

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
                    throw new Exception($"Unknown show picture place by type '{GetPlaceType()}'");
            }

            str += $", ({GetWidth()}%,{GetHeight()}%), {GetTransparency()}, ";
            switch (GetBlendMode())
            {
                case BLEND_NORMAL:
                    str += "Normal";
                    break;
                case BLEND_ADDITIVE:
                    str += context.Version == StorageVersion.MV ? "Additive" : "Add";
                    break;
                case BLEND_SUB:
                    str += context.Version == StorageVersion.MV
                        ? throw new Exception($"Unknown show picture blend mode '{GetBlendMode()}'")
                        : "Sub";
                    break;
                default:
                    throw new Exception($"Unknown show picture blend mode '{GetBlendMode()}'");
            }

            return str;
        }
    }
}
