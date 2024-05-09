using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class TransferPlayerCommand : NamedCommand
    {
        protected const int TRANSFER_DIRECT = 0;
        protected const int TRANSFER_BY_VARIABLE = 1;
        
        protected const int DIRECTION_RETAIN = 0;
        protected const int DIRECTION_DOWN = 2;
        protected const int DIRECTION_LEFT = 4;
        protected const int DIRECTION_RIGHT = 6;
        protected const int DIRECTION_UP = 8;
        
        protected const int FADE_BLACK = 0;
        protected const int FADE_NORMAL = 0; // alias
        protected const int FADE_WHITE = 1;
        protected const int FADE_NONE = 2;

        public int GetTransferType()
        {
            return GetParameter<int>(0);
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

        public int GetDirection()
        {
            return GetParameter<int>(4);
        }

        public int GetFade()
        {
            return GetParameter<int>(5);
        }

        public override int Code => CommandCodes.TransferPlayer;
        public override string GetCommandName(StorageVersion version) => "Transfer Player";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = "";
            switch (GetTransferType())
            {
                case TRANSFER_DIRECT:
                    str = context.NameFormatter.FormatMap(GetParam1());

                    if (context.Version == StorageVersion.MV)
                        str += $" ({GetParam2()},{GetParam3()})";
                    else
                        str += $" ({GetParam2():D3},{GetParam3():D3})";
                    break;
                case TRANSFER_BY_VARIABLE:
                    string var1, var2, var3;
                    if (context.Version == StorageVersion.MV)
                    {
                        var1 = context.NameFormatter.FormatVariable(GetParam1(),
                            new() { BracketType = BracketType.Figure });
                        var2 = context.NameFormatter.FormatVariable(GetParam2(),
                            new() { BracketType = BracketType.Figure });
                        var3 = context.NameFormatter.FormatVariable(GetParam3(),
                            new() { BracketType = BracketType.Figure });

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
                    throw new Exception($"Unknown transfer player transfer type '{GetTransferType()}'");
            }

            var direction = GetDirection();
            List<string> subPart = new();
            if (direction != DIRECTION_RETAIN)
            {
                var directionFmt = context.Version == StorageVersion.MV ? "Direction: {0}" : "{0}";
                subPart.Add(direction switch
                {
                    DIRECTION_DOWN => string.Format(directionFmt, "Down"),
                    DIRECTION_LEFT => string.Format(directionFmt, "Left"),
                    DIRECTION_RIGHT => string.Format(directionFmt, "Right"),
                    DIRECTION_UP => string.Format(directionFmt, "Up"),
                    _ => throw new Exception($"Unknown direction {direction}")
                });
            }

            var fade = GetFade();
            if (fade != FADE_BLACK)
            {
                var fadeFmt = context.Version == StorageVersion.MV ? "Fade: {0}" : "{0}";
                subPart.Add(fade switch
                {
                    FADE_WHITE => string.Format(fadeFmt, "White"),
                    FADE_NONE => string.Format(fadeFmt, "None"),
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
