namespace AdventOfCode.Tests;

/// <summary>
/// Base class for puzzle solver test classes.<br/>
/// Contains common tests all puzzle solver test classes want to run.
/// </summary>
/// <typeparam name="TPuzzleSolver">The <see cref="IPuzzleSolver"/> to be tested.</typeparam>
/// <typeparam name="TPuzzleSolutionData">The class containing solution data.</typeparam>
public abstract class PuzzleSolverTests<TPuzzleSolver, TPuzzleSolutionData>
    where TPuzzleSolver : IPuzzleSolver
    where TPuzzleSolutionData : IPuzzleSolutionData
{
    [Theory]
    [MemberData(nameof(GetPart1Data))]
    public void Solves_part_1_correctly(string givenInput, string correctSolution)
    {
        var solution = TPuzzleSolver.SolvePart1(givenInput);
        solution.Should().Be(correctSolution);
    }

    [Theory]
    [MemberData(nameof(GetPart2Data))]
    public void Solves_part_2_correctly(string givenInput, string correctSolution)
    {
        var solution = TPuzzleSolver.SolvePart2(givenInput);
        solution.Should().Be(correctSolution);
    }

    public static TheoryData<string, string> GetPart1Data()
        => new(TPuzzleSolutionData.Part1Data);

    public static TheoryData<string, string> GetPart2Data()
        => new(TPuzzleSolutionData.Part2Data);
}
