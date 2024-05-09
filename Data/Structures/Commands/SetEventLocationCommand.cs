using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SetEventLocationCommand : NamedCommand
    {
        protected const int TRANSFER_DIRECT = 0;
        protected const int TRANSFER_BY_VARIABLE = 1;
        protected const int TRANSFER_TYPE_EXCHANGE = 2;

        protected const int DIRECTION_RETAIN = 0;
        protected const int DIRECTION_DOWN = 2;
        protected const int DIRECTION_LEFT = 4;
        protected const int DIRECTION_RIGHT = 6;
        protected const int DIRECTION_UP = 8;

        public int GetEvent()
        {
            return GetParameter<int>(0);
        }

        public int GetTransferType()
        {
            return GetParameter<int>(1);
        }

        public int GetParam1()
        {
            return GetParameter<int>(2);
        }

        public int GetParam2()
        {
            return GetParameter<int>(3);
        }

        public int GetDirection()
        {
            return GetParameter<int>(4);
        }

        public override int Code => CommandCodes.SetEventLocation;
        public override string GetCommandName(StorageVersion version) => "Set Event Location";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatEvent(GetEvent());

            switch (GetTransferType())
            {
                case TRANSFER_DIRECT:
                    if (context.Version == StorageVersion.MV)
                        str += $", ({GetParam1()},{GetParam2()}";
                    else
                        str += $", ({GetParam1():D3},{GetParam2():D3}";
                    break;
                case TRANSFER_BY_VARIABLE:
                    string var1, var2;
                    if (context.Version == StorageVersion.MV)
                    {
                        var1 = context.NameFormatter.FormatVariable(GetParam1(),
                            new() { BracketType = BracketType.Figure });
                        var2 = context.NameFormatter.FormatVariable(GetParam2(),
                            new() { BracketType = BracketType.Figure });

                        str += $", ({var1},{var2})";
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

                        str += $", Variable {var1}{var2}";
                    }
                    break;
                case TRANSFER_TYPE_EXCHANGE:
                    if (context.Version == StorageVersion.MV)
                        str += $", Exchange with {context.NameFormatter.FormatEvent(GetParam1())}";
                    else
                        str += $", Switch with {context.NameFormatter.FormatEvent(GetParam1())}";
                    break;
                default:
                    throw new Exception($"Unknown set event location transfer type '{GetTransferType()}'");
            }

            var direction = GetDirection();
            var directionFmt = context.Version == StorageVersion.MV ? " (Direction: {0})" : ", {0}";

            str += direction switch
            {
                DIRECTION_RETAIN => "",
                DIRECTION_DOWN => string.Format(directionFmt, "Down"),
                DIRECTION_LEFT => string.Format(directionFmt, "Left"),
                DIRECTION_RIGHT => string.Format(directionFmt, "Right"),
                DIRECTION_UP => string.Format(directionFmt, "Up"),
                _ => throw new Exception($"Unknown direction {direction}")
            };

            return str;
        }
    }
}
