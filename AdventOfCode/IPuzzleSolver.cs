namespace AdventOfCode;

public interface IPuzzleSolver
{
    public static abstract PuzzleMetadata LinkedPuzzle { get; }

    public static abstract string SolvePart1(string input);
    public static abstract string SolvePart2(string input);
}
