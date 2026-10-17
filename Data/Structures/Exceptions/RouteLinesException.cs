namespace RPGMDataKit.Data.Structures.Exceptions;

/// <summary>
/// The lines after a Set Movement Route (the commands 505) are not the steps of its route (see <c>RouteLinesHelper.ValidateRouteLines</c>): their
/// number differs from the number of the steps, a line is not the step, or a line has no Set Movement Route before it.
/// </summary>
public class RouteLinesException(string message, int commandIndex, int commandCode) : CommandListException(message, commandIndex, commandCode);
