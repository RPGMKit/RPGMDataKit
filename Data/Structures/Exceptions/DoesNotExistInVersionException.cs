namespace RPGMDataKit.Data.Structures.Exceptions;

public class DoesNotExistInVersionException : Exception
{
    public DoesNotExistInVersionException(string message = "") : base(message)
    {
    }
}
