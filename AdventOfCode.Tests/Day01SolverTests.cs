namespace AdventOfCode.Tests;

public class Day01SolverTests
{
    [Fact]
    public void Solves_part_1_correctly()
    {
        const string input = """
                             3   4
                             4   3
                             2   5
                             1   3
                             3   9
                             3   3
                             """;
        const string correctSolution = "11";

        string solution = Day01Solver.SolvePart1(input);
        solution.Should().Be(correctSolution);
    }

    [Fact]
    public void Solves_part_2_correctly()
    {
        const string input = """
                             3   4
                             4   3
                             2   5
                             1   3
                             3   9
                             3   3
                             """;
        const string correctSolution = "31";

        string solution = Day01Solver.SolvePart2(input);
        solution.Should().Be(correctSolution);
    }

    [Fact]
    public void Implements_IPuzzleSolver()
    {
        typeof(Day01Solver).Should().Implement(typeof(IPuzzleSolver));
    }
}
