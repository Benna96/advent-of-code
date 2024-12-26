using System.IO.Abstractions.TestingHelpers;
using System.Net;
using Flurl.Http.Testing;

namespace AdventOfCode.Tests;

public class PuzzleInputProviderTests : IDisposable
{
    private readonly HttpTest _httpTest = new();

    private readonly string? _sessionAtStart;

    public PuzzleInputProviderTests()
    {
        _sessionAtStart = Environment.GetEnvironmentVariable(PuzzleInputProvider.SessionOptionName);
        Environment.SetEnvironmentVariable(PuzzleInputProvider.SessionOptionName, "abcd1234");
    }

    public void Dispose()
    {
        _httpTest.Dispose();
        Environment.SetEnvironmentVariable(PuzzleInputProvider.SessionOptionName, _sessionAtStart);
    }

    [Fact]
    public void Finds_existing_input()
    {
        var fileSystem = new MockFileSystem();
        var inputProvider = new PuzzleInputProvider(fileSystem);

        var puzzleWithInput = new PuzzleMetadata(Day: 1);
        var puzzlePath = inputProvider.GetPathForPuzzle(puzzleWithInput);
        var expectedInput = """
                            3   4
                            4   3
                            2   5
                            1   3
                            3   9
                            3   3

                            """;

        fileSystem.AddFile(puzzlePath, expectedInput);

        var input = inputProvider.GetInputForPuzzle(puzzleWithInput);
        input.Should().Be(expectedInput);
    }

    [Fact]
    public void Downloads_missing_input()
    {
        var puzzleWithoutInput = new PuzzleMetadata(Day: 2);
        var inputProvider = new PuzzleInputProvider(new MockFileSystem());

        inputProvider.GetInputForPuzzle(puzzleWithoutInput);
        _httpTest.ShouldHaveMadeACall();
    }

    [Fact]
    public void Fails_for_nonexistent_missing_input()
    {
        var puzzleWithoutInput = new PuzzleMetadata(Day: 2);
        var inputProvider = new PuzzleInputProvider(new MockFileSystem());
        _httpTest.RespondWith(status: (int)HttpStatusCode.NotFound);

        var act = () => inputProvider.GetInputForPuzzle(puzzleWithoutInput);
        act.Should().Throw<MissingInputException>();
    }

    [Fact]
    public void Requires_session_when_missing_input()
    {
        var puzzleWithoutInput = new PuzzleMetadata(Day: 2);
        var inputProvider = new PuzzleInputProvider(new MockFileSystem());
        Environment.SetEnvironmentVariable(PuzzleInputProvider.SessionOptionName, null);

        var act = () => inputProvider.GetInputForPuzzle(puzzleWithoutInput);
        act.Should().Throw<MissingSessionException>();
    }

    [Fact]
    public void Requires_valid_session_when_missing_input()
    {
        var puzzleWithoutInput = new PuzzleMetadata(Day: 2);
        var inputProvider = new PuzzleInputProvider(new MockFileSystem());
        _httpTest.RespondWith(status: (int)HttpStatusCode.InternalServerError);

        var act = () => inputProvider.GetInputForPuzzle(puzzleWithoutInput);
        act.Should().Throw<InvalidSessionException>();
    }
}
