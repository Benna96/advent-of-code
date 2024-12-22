namespace AdventOfCode;

public class MissingSessionException : Exception
{
    public MissingSessionException() : base()
    {
    }

    public MissingSessionException(string? message) : base(message)
    {
    }

    public MissingSessionException(string? message, Exception innerException)
        : base(message, innerException)
    {
    }
}
