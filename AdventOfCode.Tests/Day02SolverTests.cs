using System.Collections.Generic;

namespace AdventOfCode.Tests;

public class Day02SolverTests : PuzzleSolverTests<Day02Solver, Day02SolverTests>, IPuzzleSolutionData
{
    static IEnumerable<(string input, string solution)> IPuzzleSolutionData.Part1Data
        => [(Input, "2")];

    static IEnumerable<(string input, string solution)> IPuzzleSolutionData.Part2Data
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
