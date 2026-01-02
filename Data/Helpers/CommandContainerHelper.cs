using RPGMakerLibCoreV2.Data.Enums;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories;
using RPGMakerLibCoreV2.Data.Structures.Commands.Factories.MovementCommands;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Helpers;

public static class CommandContainerHelper
{
    public static IEnumerable<string> CommandsAsText<T>(ICommandContainer<T> container, EntityContext context, DumpOptions options)
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

    public static List<NamedCommand>? Unpack<T>(IEnumerable<T>? commands, StorageVersion version) where T : ICommand
    {
        if (commands == null)
            return null;

        List<NamedCommand> namedCommands = new();
        if (typeof(IMoveCommand).IsAssignableFrom(typeof(T)))
            namedCommands.AddRange(commands.Select(c => MovementCommandFactory.Create((IMoveCommand)c, version)));
        else
            namedCommands.AddRange(commands.Select(c =>
            {
                try
                {
                    return CommandFactory.Create(c, version);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }));

        return namedCommands;
    }

    public static List<T>? Pack<T>(IEnumerable<NamedCommand>? commands, StorageVersion version)
        where T : ICommand, new()
    {
        if (commands == null)
            return null;
        
        List<T> ret = new();
        ret.AddRange(commands.Select(c =>
        {
            var comm = new T() { Code = c.Code, Indent = c.Indent };

            c._Pack(comm, version);
            if (comm.Parameters == null && (comm.Code == 0 || comm.Indent != null))
                comm.Parameters = [];

            return comm;
        }));

        return ret;
    }
}
