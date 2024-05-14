using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class CommandContainerHelper
{
    public static IEnumerable<string> CommandsAsText(ICommandContainer container, EntityContext context, DumpOptions options)
    {
        if (options.Format == DumpFormat.Text)
        {
            foreach (var command in container.NamedCommands)
                yield return command.AsString(context, options);
            yield break;
        }

        throw new Exception($"Dump format {options.Format} not implemented");
    }
}
