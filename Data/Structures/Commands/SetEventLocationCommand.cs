using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class SetEventLocationCommand : NamedCommand
    {
        public enum DesignationType
        {
            Direct = 0,
            ByVariable = 1,
            Exchange = 2,
        }

        public int GetEvent()
        {
            return GetParameter<int>(0);
        }

        public DesignationType GetDesignationType()
        {
            return (DesignationType)GetParameter<int>(1);
        }

        public int GetParam1()
        {
            return GetParameter<int>(2);
        }

        public int GetParam2()
        {
            return GetParameter<int>(3);
        }

        public Direction GetDirection()
        {
            return (Direction)GetParameter<int>(4);
        }

        public override int Code => CommandCodes.SetEventLocation;
        public override string GetCommandName(StorageVersion version) => "Set Event Location";

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var str = context.NameFormatter.FormatEvent(GetEvent());

            switch (GetDesignationType())
            {
                case DesignationType.Direct:
                    if (context.Version == StorageVersion.MV)
                        str += $", ({GetParam1()},{GetParam2()}";
                    else
                        str += $", ({GetParam1():D3},{GetParam2():D3}";
                    break;
                case DesignationType.ByVariable:
                    string var1, var2;
                    if (context.Version == StorageVersion.MV)
                    {
                        var1 = context.NameFormatter.FormatVariable(GetParam1());
                        var2 = context.NameFormatter.FormatVariable(GetParam2());

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
                case DesignationType.Exchange:
                    if (context.Version == StorageVersion.MV)
                        str += $", Exchange with {context.NameFormatter.FormatEvent(GetParam1())}";
                    else
                        str += $", Switch with {context.NameFormatter.FormatEvent(GetParam1())}";
                    break;
                default:
                    throw new Exception($"Unknown set event location transfer type '{GetDesignationType()}'");
            }

            var direction = GetDirection();
            var directionFmt = context.Version == StorageVersion.MV ? " (Direction: {0})" : ", {0}";

            str += direction switch
            {
                Direction.Retain => "",
                Direction.Down => string.Format(directionFmt, "Down"),
                Direction.Left => string.Format(directionFmt, "Left"),
                Direction.Right => string.Format(directionFmt, "Right"),
                Direction.Up => string.Format(directionFmt, "Up"),
                _ => throw new Exception($"Unknown direction {direction}")
            };

            return str;
        }
    }
}
