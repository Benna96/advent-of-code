using System.IO.Abstractions.TestingHelpers;
using System.Net;
using Flurl.Http.Testing;

namespace AdventOfCode.Tests;

public class PuzzleInputProviderTests : IDisposable
{
    private readonly HttpTest _httpTest;
    private readonly MockFileSystem _fileSystem;
    private readonly PuzzleInputProvider _inputProvider;

    private readonly string? _sessionAtStart;

    public PuzzleInputProviderTests()
    {
        _httpTest = new HttpTest();
        _fileSystem = new MockFileSystem();
        _inputProvider = new PuzzleInputProvider(_fileSystem);

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
        var puzzleWithInput = new PuzzleMetadata(Day: 1);
        var puzzlePath = _inputProvider.GetPathForPuzzle(puzzleWithInput);
        var expectedInput = """
                            3   4
                            4   3
                            2   5
                            1   3
                            3   9
                            3   3

                            """;

        _fileSystem.AddFile(puzzlePath, expectedInput);

        var input = _inputProvider.GetInputForPuzzle(puzzleWithInput);
        input.Should().Be(expectedInput);
    }

    [Fact]
    public void Downloads_missing_input()
    {
        var puzzleWithoutInput = new PuzzleMetadata(Day: 2);

        _inputProvider.GetInputForPuzzle(puzzleWithoutInput);
        _httpTest.ShouldHaveMadeACall();
    }

    [Fact]
    public void Fails_for_nonexistent_missing_input()
    {
        var puzzleWithoutInput = new PuzzleMetadata(Day: 2);
        _httpTest.RespondWith(status: (int)HttpStatusCode.NotFound);

        var act = () => _inputProvider.GetInputForPuzzle(puzzleWithoutInput);
        act.Should().Throw<MissingInputException>();
    }

    [Fact]
    public void Requires_session_when_missing_input()
    {
        var puzzleWithoutInput = new PuzzleMetadata(Day: 2);
        Environment.SetEnvironmentVariable(PuzzleInputProvider.SessionOptionName, null);

        var act = () => _inputProvider.GetInputForPuzzle(puzzleWithoutInput);
        act.Should().Throw<MissingSessionException>();
    }

    [Fact]
    public void Requires_valid_session_when_missing_input()
    {
        var puzzleWithoutInput = new PuzzleMetadata(Day: 2);
        _httpTest.RespondWith(status: (int)HttpStatusCode.InternalServerError);

        var act = () => _inputProvider.GetInputForPuzzle(puzzleWithoutInput);
        act.Should().Throw<InvalidSessionException>();
    }
}
