using RPGMDataKit.Data.Enums;
using RPGMDataKit.Data.Structures.Commands;
using RPGMDataKit.Data.Structures.Exceptions;
using RPGMDataKit.Data.Structures.Interfaces;
using RPGMDataKit.Data.Structures.Internal;

namespace RPGMDataKit.Data.Helpers;

/// <summary>
/// The lines after Set Movement Route. The route is in the command (the engines run it from there); the editors also write a line (the command 505)
/// for each step of it, one after another right after the command, and show the route from them. So the route is what counts and the lines
/// are made from it: change the steps of <c>SetMovementRouteCommand.Route</c>, then call <see cref="SetupRouteLines"/>. A line is not changed
/// by itself (the next <see cref="SetupRouteLines"/> writes it again from the route).
/// </summary>
public static class RouteLinesHelper
{
    /// <summary>
    /// Writes the lines after each Set Movement Route again from its route: one for each step, none for the empty command that ends the route.
    /// A line that has no Set Movement Route before it is removed. The indent of a line is the one of its command.
    /// </summary>
    /// <returns>The number of the lines that were removed and added.</returns>
    public static int SetupRouteLines(ICommandContainer container)
    {
        var commands = container.NamedCommands;
        var changed = 0;
        for (var i = 0; i < commands.Count; i++)
        {
            if (commands[i] is not SetMovementRouteCommand move)
            {
                if (commands[i].Code == CommandCodes.MovementRouteCommand)
                {
                    commands.RemoveAt(i--);
                    ++changed;
                }

                continue;
            }

            var next = i + 1;
            while (next < commands.Count && commands[next].Code == CommandCodes.MovementRouteCommand)
            {
                commands.RemoveAt(next);
                ++changed;
            }

            foreach (var step in Steps(move))
            {
                commands.Insert(next++, new MovementRouteCommand { Indent = move.Indent, MoveCommand = step.Clone() });
                ++changed;
            }

            i = next - 1;
        }

        return changed;
    }

    /// <summary>Throws a <see cref="RouteLinesException"/> if the lines after a Set Movement Route are not the steps of its route.</summary>
    public static void ValidateRouteLines(ICommandContainer container)
    {
        var commands = container.NamedCommands;
        for (var i = 0; i < commands.Count; i++)
        {
            if (commands[i] is not SetMovementRouteCommand move)
            {
                if (commands[i].Code == CommandCodes.MovementRouteCommand)
                    throw new RouteLinesException($"Command 505 at index {i} has no Set Movement Route before it", i, commands[i].Code);

                continue;
            }

            var steps = Steps(move).ToList();
            for (var s = 0; s < steps.Count; s++)
            {
                var at = i + 1 + s;
                if (at >= commands.Count || commands[at] is not MovementRouteCommand line)
                    throw new RouteLinesException($"Set Movement Route at index {i} has {steps.Count} steps, and {s} lines after it", i, move.Code);

                if (!Same(line.MoveCommand, steps[s]))
                    throw new RouteLinesException($"Command 505 at index {at} is not the step {s} of the route at index {i}", at, line.Code);
            }

            var after = i + 1 + steps.Count;
            if (after < commands.Count && commands[after].Code == CommandCodes.MovementRouteCommand)
                throw new RouteLinesException($"Set Movement Route at index {i} has {steps.Count} steps, and more lines after it", after, commands[after].Code);

            i += steps.Count;
        }
    }

    // the steps that get a line: all but the empty command that ends the route
    private static IEnumerable<NamedCommand> Steps(SetMovementRouteCommand move) => move.Route.NamedCommands.Where(c => c.Code != 0);

    private static bool Same(NamedCommand line, NamedCommand step) =>
        line.Code == step.Code && SameValues(line.Raw.Parameters ?? [], step.Raw.Parameters ?? []);

    private static bool SameValues(object? a, object? b)
    {
        switch (a, b)
        {
            case (null, null):
                return true;
            case (Enum or int, Enum or int):
                return Convert.ToInt32(a) == Convert.ToInt32(b);
            case (IAudioFile sa, IAudioFile sb):
                return sa.Name == sb.Name && sa.Volume == sb.Volume && sa.Pitch == sb.Pitch
                       && (!sa.HasProperty(nameof(sa.Pan)) || !sb.HasProperty(nameof(sb.Pan)) || sa.Pan == sb.Pan);
            case (System.Collections.IList la, System.Collections.IList lb):
                return la.Count == lb.Count && la.Cast<object?>().Zip(lb.Cast<object?>(), SameValues).All(same => same);
            default:
                return Equals(a, b) || a?.ToString() == b?.ToString();
        }
    }
}
