namespace AdventOfCode;

public record PuzzleMetadata(int Day)
{
    public int Day { get; }
        = Day is < 1 or > 25
            ? throw new ArgumentOutOfRangeException(nameof(Day), Day, $"Value must be between 1 and 25")
            : Day;
}
