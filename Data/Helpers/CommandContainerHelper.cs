using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories.MovementCommands;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class CommandContainerHelper
{
    public static IEnumerable<string> CommandsAsText(ICommandContainer container, EntityContext context, DumpOptions options)
    {
        if (options.Format == DumpFormat.Text)
        {
            var i = 0;
            foreach (var command in container.NamedCommands)
            {
                var lineNumber = "";
                if (!string.IsNullOrWhiteSpace(options.LineNumberFormat))
                    lineNumber = string.Format("{0:" + options.LineNumberFormat + "} ", i);

                ++i;

                yield return lineNumber + command.AsString(context, options);
            }

            yield break;
        }

        throw new Exception($"Dump format {options.Format} not implemented");
    }

    /// <summary>The named command (a view) over a raw command.</summary>
    public static NamedCommand CreateNamed<T>(T command, StorageVersion version) where T : ICommand
    {
        if (typeof(IMoveCommand).IsAssignableFrom(typeof(T)))
            return MovementCommandFactory.Create((IMoveCommand)command, version);

        return CommandFactory.Create(command, version);
    }
}
