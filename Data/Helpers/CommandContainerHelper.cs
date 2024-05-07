using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class CommandContainerHelper
{
    public static string Dump(ICommandContainer container, EntityContext context, DumpOptions? options = null)
    {
        if (options == null)
            options = new();

        if (options.Format == DumpFormat.Text)
            return string.Join("\r\n", container.NamedCommands.Select(_ => _.AsString(context, options)));

        throw new Exception($"Dump format {options.Format} not implemented");
    }
}
