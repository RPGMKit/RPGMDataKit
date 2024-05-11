using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class TransferPlayerCommand : NamedCommand
    {
        public enum DesignationType
        {
            Direct = 0,
            ByVariable = 1,
        }

        public enum FadeType
        {
            Black = 0,
            Normal = 0,
            White = 1,
            None = 2,
        }

        public DesignationType GetDesignationType()
        {
            return (DesignationType)GetParameter<int>(0);
        }

        public int GetParam1()
        {
            return GetParameter<int>(1);
        }

        public int GetParam2()
        {
            return GetParameter<int>(2);
        }

        public int GetParam3()
        {
            return GetParameter<int>(3);
        }

        public Direction GetDirection()
        {
            return (Direction)GetParameter<int>(4);
        }

        public FadeType GetFade()
        {
            return (FadeType)GetParameter<int>(5);
        }

        public override int Code => CommandCodes.TransferPlayer;
        public override string GetCommandName(StorageVersion version) => "Transfer Player";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = "";
            switch (GetDesignationType())
            {
                case DesignationType.Direct:
                    str = context.NameFormatter.FormatMap(GetParam1());

                    if (context.Version == StorageVersion.MV)
                        str += $" ({GetParam2()},{GetParam3()})";
                    else
                        str += $" ({GetParam2():D3},{GetParam3():D3})";
                    break;
                case DesignationType.ByVariable:
                    string var1, var2, var3;
                    if (context.Version == StorageVersion.MV)
                    {
                        var1 = context.NameFormatter.FormatVariable(GetParam1());
                        var2 = context.NameFormatter.FormatVariable(GetParam2());
                        var3 = context.NameFormatter.FormatVariable(GetParam3());

                        str = $"{var1} ({var2},{var3})";
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
                        var3 = context.NameFormatter.FormatVariable(GetParam3(), new()
                        {
                            SkipTitle = true,
                            SkipName = true,
                        });

                        str = $"Variable {var1}{var2}{var3}";
                    }

                    break;
                default:
                    throw new Exception($"Unknown transfer player transfer type '{GetDesignationType()}'");
            }

            var direction = GetDirection();
            List<string> subPart = new();
            if (direction != Direction.Retain)
            {
                var directionFmt = context.Version == StorageVersion.MV ? "Direction: {0}" : "{0}";
                subPart.Add(direction switch
                {
                    Direction.Down => string.Format(directionFmt, "Down"),
                    Direction.Left => string.Format(directionFmt, "Left"),
                    Direction.Right => string.Format(directionFmt, "Right"),
                    Direction.Up => string.Format(directionFmt, "Up"),
                    _ => throw new Exception($"Unknown direction {direction}")
                });
            }

            var fade = GetFade();
            if (fade != FadeType.Black)
            {
                var fadeFmt = context.Version == StorageVersion.MV ? "Fade: {0}" : "{0}";
                subPart.Add(fade switch
                {
                    FadeType.White => string.Format(fadeFmt, "White"),
                    FadeType.None => string.Format(fadeFmt, "None"),
                    _ => throw new Exception($"Unknown fade '{fade}'")
                });
            }

            if (subPart.Count > 0)
            {
                var joined = string.Join(", ", subPart);
                if (context.Version == StorageVersion.MV)
                    str += $" ({joined})";
                else
                    str += $", {joined}";
            }

            return str;
        }
    }
}
