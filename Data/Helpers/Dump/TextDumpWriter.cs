using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Helpers.Dump;

public sealed class TextDumpWriter : IDumpWriter
{
    public async Task Write(TextWriter writer, string header, ICommandContainer container, EntityContext context, DumpOptions options)
    {
        if (options.ExpandCommonEvents)
            throw new NotSupportedException("The text dump cannot expand the common events");

        await writer.WriteAsync(header);
        await writer.WriteLineAsync("Commands:");
        foreach (var line in CommandContainerHelper.CommandsAsText(container, context, options))
            await writer.WriteLineAsync(line);
    }
}
