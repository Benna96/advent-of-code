using System.Collections.Generic;

namespace AdventOfCode.Tests;

public class Day01SolverTests : PuzzleSolverTests<Day01Solver, Day01SolverTests>, IPuzzleSolutionData
{
    static IEnumerable<(string input, string solution)> IPuzzleSolutionData.Part1Data
        => [(Input, "11")];

    static IEnumerable<(string input, string solution)> IPuzzleSolutionData.Part2Data
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
