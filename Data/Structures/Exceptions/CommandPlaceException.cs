namespace RPGMDataKit.Data.Structures.Exceptions;

/// <summary>
/// Commands cannot be put before, replaced or removed at this index (see <c>CommandContainerExtensions</c>): it is a line of a command of several
/// lines, what divides or ends a block, or (to replace or remove) the empty command that ends a block or the list. The message says where to go
/// instead.
/// </summary>
public class CommandPlaceException(string message, int commandIndex, int commandCode) : CommandListException(message, commandIndex, commandCode);
