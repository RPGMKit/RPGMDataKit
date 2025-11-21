using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2;

public interface IDumpable
{
    public Task Dump(TextWriter writer, EntityContext context, DumpOptions options);
}