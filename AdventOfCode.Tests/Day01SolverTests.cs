namespace AdventOfCode.Tests;

public class Day01SolverTests : PuzzleSolverTestsBase<Day01SolverTests, Day01Solver>, IPuzzleSolverTests
{
    static TheoryData<(string input, string solution)> IPuzzleSolverTests.Part1TestCases
        => [(Input, "11")];

    static TheoryData<(string input, string solution)> IPuzzleSolverTests.Part2TestCases
        => [(Input, "31")];

    private const string Input = """
                                 3   4
                                 4   3
                                 2   5
                                 1   3
                                 3   9
                                 3   3
                                 """;
}
