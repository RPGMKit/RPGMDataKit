using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit;

public interface IDumpable
{
    public Task Dump(TextWriter writer, EntityContext context, DumpOptions options);
}