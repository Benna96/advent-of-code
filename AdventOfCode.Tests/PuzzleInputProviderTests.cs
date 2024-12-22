using System.IO;
using System.Linq;
using System.Net;
using Flurl.Http.Testing;

namespace AdventOfCode.Tests;

public class PuzzleInputProviderTests : IDisposable
{
    private readonly HttpTest _httpTest = new();

    private readonly string? _sessionAtStart;

    private readonly string[] _inputFilesAtStart
        = Directory.GetFiles(PuzzleInputProvider.InputFolder);

    public PuzzleInputProviderTests()
    {
        _sessionAtStart = Environment.GetEnvironmentVariable(PuzzleInputProvider.SessionOptionName);
        Environment.SetEnvironmentVariable(PuzzleInputProvider.SessionOptionName, "abcd1234");
    }

    public void Dispose()
    {
        _httpTest.Dispose();
        Environment.SetEnvironmentVariable(PuzzleInputProvider.SessionOptionName, _sessionAtStart);

        var inputFilesAtEnd = Directory.GetFiles(PuzzleInputProvider.InputFolder);
        foreach (var inputFile in inputFilesAtEnd)
        {
            if (!_inputFilesAtStart.Contains(inputFile))
                File.Delete(inputFile);
        }
    }

    [Fact]
    public void Finds_existing_input()
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
    public void Downloads_missing_input()
    {
        var puzzleWithoutInput = new PuzzleMetadata(Day: 2);

        PuzzleInputProvider.GetInputForPuzzle(puzzleWithoutInput);
        _httpTest.ShouldHaveMadeACall();
    }

    [Fact]
    public void Fails_for_nonexistent_missing_input()
    {
        var puzzleWithoutInput = new PuzzleMetadata(Day: 2);
        _httpTest.RespondWith(status: (int)HttpStatusCode.NotFound);

        var act = () => PuzzleInputProvider.GetInputForPuzzle(puzzleWithoutInput);
        act.Should().Throw<MissingInputException>();
    }

    [Fact]
    public void Requires_session_when_missing_input()
    {
        var puzzleWithoutInput = new PuzzleMetadata(Day: 2);
        Environment.SetEnvironmentVariable(PuzzleInputProvider.SessionOptionName, null);

        var act = () => PuzzleInputProvider.GetInputForPuzzle(puzzleWithoutInput);
        act.Should().Throw<MissingSessionException>();
    }

    [Fact]
    public void Requires_valid_session_when_missing_input()
    {
        var puzzleWithoutInput = new PuzzleMetadata(Day: 2);
        _httpTest.RespondWith(status: (int)HttpStatusCode.InternalServerError);

        var act = () => PuzzleInputProvider.GetInputForPuzzle(puzzleWithoutInput);
        act.Should().Throw<InvalidSessionException>();
    }
}
