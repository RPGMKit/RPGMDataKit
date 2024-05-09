namespace RPGMakerLibCoreV2.Data.Structures.Exceptions;

public class DoesNotExistInVersionException : Exception
{
    public DoesNotExistInVersionException(string message = "") : base(message)
    {
    }
}
