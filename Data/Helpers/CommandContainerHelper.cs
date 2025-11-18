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

    public static List<NamedCommand> Unpack<T>(IEnumerable<T>? commands, StorageVersion version) where T : ICommand
    {
        List<NamedCommand> namedCommands = new();
        if (commands != null)
        {
            if (typeof(ICommandWithIndent).IsAssignableFrom(typeof(T)))
                namedCommands.AddRange(commands.Select(c =>
                {
                    try
                    {
                        return CommandFactory.Create((ICommandWithIndent)c, version);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }));
            else if (typeof(IMoveCommand).IsAssignableFrom(typeof(T)))
                namedCommands.AddRange(commands.Select(c => MovementCommandFactory.Create((IMoveCommand)c, version)));
            else
                throw new Exception($"Unsupported Command type {typeof(T).Name}");
        }

        return namedCommands;
    }

    public static LookupList<T>? Pack<T>(IEnumerable<NamedCommand>? commands, StorageVersion version)
        where T : ICommand, new()
    {
        if (commands == null)
            return null;
        
        LookupList<T> ret = new();
        ret.AddRange(commands.Select(_ =>
        {
            var comm = new T() { Code = _.Code };

            if (comm is ICommandWithIndent commandWithIndent)
                commandWithIndent.Indent = _.Indent;

            var parameters = _._Pack(version);

            comm.Parameters = parameters.Count > 0 ? parameters : [];

            return comm;
        }));

        return ret;
    }
}
