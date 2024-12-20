namespace AdventOfCode.Tests;

public class Day02SolverTests : PuzzleSolverTestsBase<Day02SolverTests, Day02Solver>, IPuzzleSolverTests
{
    static TheoryData<(string input, string solution)> IPuzzleSolverTests.Part1TestCases
        => [(Input, "2")];

    static TheoryData<(string input, string solution)> IPuzzleSolverTests.Part2TestCases
        => [(Input, "4")];

    private const string Input = """
                                 7 6 4 2 1
                                 1 2 7 8 9
                                 9 7 6 2 1
                                 1 3 2 4 5
                                 8 6 4 4 1
                                 1 3 6 7 9
                                 """;
}
