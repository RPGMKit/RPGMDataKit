using RPGMakerLibCoreV2.Data.Structures.Exceptions;
using RPGMakerLibCoreV2.Data.Structures.Interfaces;
using RPGMakerLibCoreV2.Data.Structures.Internal;

namespace RPGMakerLibCoreV2.Data.Structures.Commands.Helpers;

internal static class Copies
{
    /// <summary>
    /// A copy of a value of a command property that shares nothing with the original: objects (a sound, a tone, a color,
    /// a route, a list of choices, a nested command) are copied, everything else is a value and is returned as it is.
    /// </summary>
    public static object? Of(object? value)
    {
        switch (value)
        {
            case null:
                return null;

            case IAudioFile sound:
            {
                var copy = (IAudioFile)Activator.CreateInstance(sound.GetType())!;
                copy.Name = sound.Name;
                copy.Volume = sound.Volume;
                copy.Pitch = sound.Pitch;
                try
                {
                    copy.Pan = sound.Pan;
                }
                catch (DoesNotExistInVersionException)
                {
                    // VXAce sounds have no pan
                }

                return copy;
            }

            case ITone tone:
            {
                var copy = (ITone)Activator.CreateInstance(tone.GetType())!;
                copy.Red = tone.Red;
                copy.Green = tone.Green;
                copy.Blue = tone.Blue;
                copy.Gray = tone.Gray;
                return copy;
            }

            case IColor color:
            {
                var copy = (IColor)Activator.CreateInstance(color.GetType())!;
                copy.Red = color.Red;
                copy.Green = color.Green;
                copy.Blue = color.Blue;
                copy.Alpha = color.Alpha;
                return copy;
            }

            case IMoveRoute route:
            {
                var copy = (IMoveRoute)Activator.CreateInstance(route.GetType())!;
                copy.Repeat = route.Repeat;
                copy.Skippable = route.Skippable;
                copy.Wait = route.Wait;
                copy.NamedCommands = route.NamedCommands.Select(command => command.Clone()).ToList();
                return copy;
            }

            case NamedCommand command:
                return command.Clone();

            case List<string> list:
                return new List<string>(list);

            default:
                return value;
        }
    }
}
