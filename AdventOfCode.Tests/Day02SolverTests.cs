namespace AdventOfCode.Tests;

public class Day02SolverTests
{
    [Fact]
    public void Solves_part_1_correctly()
    {
        const string input = """
                             7 6 4 2 1
                             1 2 7 8 9
                             9 7 6 2 1
                             1 3 2 4 5
                             8 6 4 4 1
                             1 3 6 7 9
                             """;
        const string correctSolution = "2";

        string solution = Day02Solver.SolvePart1(input);
        solution.Should().Be(correctSolution);
    }

    [Fact]
    public void Solves_part_2_correctly()
    {
        const string input = """
                             7 6 4 2 1
                             1 2 7 8 9
                             9 7 6 2 1
                             1 3 2 4 5
                             8 6 4 4 1
                             1 3 6 7 9
                             """;
        const string correctSolution = "4";

        string solution = Day02Solver.SolvePart2(input);
        solution.Should().Be(correctSolution);
    }

    [Fact]
    public void Implements_IPuzzleSolver()
    {
        typeof(Day02Solver).Should().Implement(typeof(IPuzzleSolver));
    }
}
