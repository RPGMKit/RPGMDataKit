using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands
{
    public class ShowTextBaseCommand : NamedCommand
    {
        public enum BackgroundType
        {
            Window = 0,
            Dim = 1,
            Transparent = 2,
        }

        public enum PositionType
        {
            Top = 0,
            Middle = 1,
            Bottom = 2,
        }
        
        public override int Code => CommandCodes.ShowTextBase;
        public override string GetCommandName(StorageVersion version) => "Text";

        public string GetFace()
        {
            return GetParameter<string>(0);
        }

        public int GetOffset()
        {
            return GetParameter<int>(1);
        }

        public BackgroundType GetBackground()
        {
            return (BackgroundType)GetParameter<int>(2);
        }

        public PositionType GetPosition()
        {
            return (PositionType)GetParameter<int>(3);
        }

        public override string GetParametersDescription(EntityContext context, DumpOptions options)
        {
            var face = GetFace();
            if (string.IsNullOrWhiteSpace(face))
                face = context.Version == StorageVersion.MV ? "None" : "-, -";
            else
                face = context.Version == StorageVersion.MV ? $"{face}({GetOffset()})" : $"'{face}', {GetOffset()}";

            var background = "";
            switch (GetBackground())
            {
                case BackgroundType.Window:
                    background = context.Version == StorageVersion.MV ? "Window" : "Normal";
                    break;
                case BackgroundType.Dim:
                    background = context.Version == StorageVersion.MV ? "Dim" : "Dark";
                    break;
                case BackgroundType.Transparent:
                    background = "Transparent";
                    break;
                default:
                    throw new Exception($"Unknown background '{GetBackground()}'");
            }

            var position = "";
            switch (GetPosition())
            {
                case PositionType.Top:
                    position = "Top";
                    break;
                case PositionType.Middle:
                    position = "Middle";
                    break;
                case PositionType.Bottom:
                    position = "Bottom";
                    break;
                default:
                    throw new Exception($"Unknown position '{GetPosition()}'");
            }

            return $"{face}, {background}, {position}";
        }
    }
}
