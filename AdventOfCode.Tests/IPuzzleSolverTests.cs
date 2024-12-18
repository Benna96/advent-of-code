namespace AdventOfCode.Tests;

public interface IPuzzleSolverTests
{
    public static abstract TheoryData<(string input, string solution)> Part1TestCases { get; }
    public static abstract TheoryData<(string input, string solution)> Part2TestCases { get; }
}
