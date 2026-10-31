using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Helpers.Dump;

/// <summary>Writes a dumped entity in one format: its header (the lines the entity gives) and the commands of its list.</summary>
public interface IDumpWriter
{
    Task Write(TextWriter writer, string header, ICommandContainer container, EntityContext context, DumpOptions options);
}
