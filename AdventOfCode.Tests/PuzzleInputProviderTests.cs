namespace AdventOfCode.Tests;

public class PuzzleInputProviderTests
{
    [Fact]
    public void Finds_existing_puzzle_input()
    {
        var puzzleWithInput = new PuzzleMetadata(Day: 1);
        var expectedInput = """
                            3   4
                            4   3
                            2   5
                            1   3
                            3   9
                            3   3

                            """;

        var input = PuzzleInputProvider.GetInputForPuzzle(puzzleWithInput);
        input.Should().Be(expectedInput);
    }

    [Fact]
    public void Throws_when_missing_puzzle_input()
    {
        var puzzleWithoutInput = new PuzzleMetadata(Day: 2);

        var act = () => PuzzleInputProvider.GetInputForPuzzle(puzzleWithoutInput);
        act.Should().Throw<MissingInputException>();
    }
}
