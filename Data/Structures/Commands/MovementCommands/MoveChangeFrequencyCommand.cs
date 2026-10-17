using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands.MovementCommands;

public class MoveChangeFrequencyCommand : AbstractMoveCommand
{
    public override int Code => MovementRouteCommandCodes.ChangeFrequency;
    public override string GetCommandName(StorageVersion version) => "Frequency";

    public enum FrequencyType
    {
        Lowest = 1,
        Lower = 2,
        Normal = 3,
        Higher = 4,
        Highest = 5,
    }
    
    public FrequencyType Frequency
    {
        get => Get<FrequencyType>(0);
        set => Set(0, value);
    }

    // a new command has the normal frequency in the editors
    protected override void OnBinding(StorageVersion version)
    {
        if (!IsParameterSet(0))
            Set(0, FrequencyType.Normal);
    }

    // the number, as the editors show it
    public override string GetParametersDescription(EntityContext context, DumpOptions options) => ((int)Frequency).ToString();
}
