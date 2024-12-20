namespace AdventOfCode.Tests;

/// <summary>
/// Base class for puzzle solver test classes.<br/>
/// Contains common tests all puzzle solver test classes want to run.
/// </summary>
/// <typeparam name="TPuzzleSolverTests">The implementing class; Self-type.</typeparam>
/// <typeparam name="TPuzzleSolver">The <see cref="IPuzzleSolver"/> to be tested.</typeparam>
public abstract class PuzzleSolverTestsBase<TPuzzleSolverTests, TPuzzleSolver>
    where TPuzzleSolverTests : PuzzleSolverTestsBase<TPuzzleSolverTests, TPuzzleSolver>, IPuzzleSolverTests
    where TPuzzleSolver : IPuzzleSolver
{
    [Theory]
    [MemberData(nameof(GetPart1TestCases))]
    public void Solves_part_1_correctly((string givenInput, string correctSolution) data)
    {
        var solution = TPuzzleSolver.SolvePart1(data.givenInput);
        solution.Should().Be(data.correctSolution);
    }

    [Theory]
    [MemberData(nameof(GetPart2TestCases))]
    public void Solves_part_2_correctly((string givenInput, string correctSolution) data)
    {
        var solution = TPuzzleSolver.SolvePart2(data.givenInput);
        solution.Should().Be(data.correctSolution);
    }

    public static TheoryData<(string input, string solution)> GetPart1TestCases()
        => TPuzzleSolverTests.Part1TestCases;

    public static TheoryData<(string input, string solution)> GetPart2TestCases()
        => TPuzzleSolverTests.Part2TestCases;
}
