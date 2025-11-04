using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Helpers;
using RPGMakerLibCoreV2.Storages.Abstraction;

namespace RPGMakerLibCoreV2.Data.Structures.Internal;

public class EntityContext
{
    public EntityContext(Storage storage, bool extendedFormat = false)
    {
        Storage = storage;
        NameFormatter = new(this, extendedFormat);
    }
    
    public EntityContext(Storage storage, CommonEventContext context, bool extendedFormat = false) : this(storage, extendedFormat)
    {
        CommonEventContext = context;
    }
    
    public EntityContext(Storage storage, MapEventContext context, bool extendedFormat = false) : this(storage, extendedFormat)
    {
        MapEventContext = context;
    }
    
    public EntityContext(Storage storage, TroopContext context, bool extendedFormat = false) : this(storage, extendedFormat)
    {
        TroopContext = context;
    }

    public Storage Storage { get; }
    
    public StorageVersion Version => Storage.Version;

    public CommonEventContext? CommonEventContext { get; set; }

    public MapEventContext? MapEventContext { get; set; }

    public TroopContext? TroopContext { get; set; }

    public NameFormatter NameFormatter { get; private set; }

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
