namespace AdventOfCode;

public static class SampleFormatter
{
    public static string Format(int year)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(year);

        return $"Advent of Code {year}";
    }
}
