using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public class CommandContext
{
    public Storage Storage { get; set; }
    
    public StorageVersion Version => Storage.Version;
    
    public CommonEventContext? CommonEventContext { get; set; }
    
    public MapEventContext? MapEventContext { get; set; }
    
    public TroopContext? TroopContext { get; set; }

    public override string ToString()
    {
        if (CommonEventContext != null)
            return CommonEventContext.ToString();
        if (MapEventContext != null)
            return MapEventContext.ToString();
        if (TroopContext != null)
            return TroopContext.ToString();

        return "Empty";
    }
}
