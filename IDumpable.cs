using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2;

public interface IDumpable
{
    public void Dump(TextWriter writer, EntityContext context, DumpOptions options);
}