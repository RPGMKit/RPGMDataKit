using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2;

public interface IDumpable
{
    public string Dump(EntityContext context, DumpOptions options);
}