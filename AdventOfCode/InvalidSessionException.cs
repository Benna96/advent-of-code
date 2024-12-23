namespace AdventOfCode;

public class InvalidSessionException : Exception
{
    public InvalidSessionException() : base()
    {
    }

    public InvalidSessionException(string? message) : base(message)
    {
    }

    public InvalidSessionException(string? message, Exception innerException)
        : base(message, innerException)
    {
    }
}
