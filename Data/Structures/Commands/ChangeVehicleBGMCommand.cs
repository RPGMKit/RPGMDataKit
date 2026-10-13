using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands.Abstraction;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Structures.Commands;

public class ChangeVehicleBGMCommand : NamedCommand
{
    public override int Code => CommandCodes.ChangeVehicleBGM;
    public override string GetCommandName(StorageVersion version) => "Change Vehicle BGM";

    public VehicleType VehicleType
    {
        get => Get<VehicleType>(0);
        set => Set(0, value);
    }

    public IBGM SoundData
    {
        get => GetEntity<IBGM>(1);
        set => Set(1, value);
    }

    protected override void OnMadeInCodeBound(StorageVersion version)
    {
        if (version != StorageVersion.MV)
            throw new Exception("Exists only in MV");
    }

    public override string GetParametersDescription(EntityContext context, DumpOptions options)
    {
        var str = VehicleType switch
        {
            VehicleType.Boat => "Boat",
            VehicleType.Ship => "Ship",
            VehicleType.Airship => "Airship",
            _ => throw new Exception($"Unknown vehicle type: {VehicleType}")
        };
        
        return str + ", " + context.NameFormatter.FormatSoundData(SoundData);
    }
}
