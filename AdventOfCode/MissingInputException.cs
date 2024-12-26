namespace AdventOfCode;

public class MissingInputException : Exception
{
    public MissingInputException() : base()
    {
    }

    public MissingInputException(string? message) : base(message)
    {
    }

    public MissingInputException(string? message, Exception innerException)
        : base(message, innerException)
    {
    }
}
