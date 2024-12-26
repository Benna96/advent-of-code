using Flurl;
using Flurl.Http;
using System.IO.Abstractions;
using System.Net;

namespace AdventOfCode;

public class PuzzleInputProvider
{
    public static readonly string SessionOptionName = "AOC_SESSION";

    public static PuzzleInputProvider Instance { get; } = new();

    private readonly IFileSystem _fileSystem;

    private PuzzleInputProvider() : this(new FileSystem())
    {
    }

    /// <summary>
    /// Only tests should use this! Elsewhere, just use <see cref="Instance"/>.
    /// </summary>
    /// <param name="fileSystem"></param>
    public PuzzleInputProvider(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public string GetPathForPuzzle(PuzzleMetadata puzzle)
    {
        var zeroPadded = $"{puzzle.Day:00}";
        var path = _fileSystem.Path.Join("Inputs", $"{zeroPadded}.txt");
        return path;
    }

    public string GetInputForPuzzle(PuzzleMetadata puzzle)
    {
        var path = GetPathForPuzzle(puzzle);
        if (!_fileSystem.File.Exists(path))
        {
            DownloadPuzzleInputTo(puzzle, path);
        }

        return _fileSystem.File.ReadAllText(path);
    }

    private void DownloadPuzzleInputTo(PuzzleMetadata puzzle, string path)
    {
        var session = Environment.GetEnvironmentVariable(SessionOptionName);
        if (string.IsNullOrEmpty(session))
        {
            throw new MissingSessionException(
                "Couldn't find session cookie! " +
                "See README for how to provide it.");
        }

        var request = "https://adventofcode.com"
            .AppendPathSegments(2024, "day", puzzle.Day, "input")
            .WithHeader(
                "User-Agent",
                ".NET/9.0 (https://github.com/Benna96/advent-of-code by Benna96, vissersofia@gmail.com)")
            .WithCookie("session", session);

        try
        {
            request.DownloadFileAsync(_fileSystem, path)
                .GetAwaiter()
                .GetResult();
        }
        catch (FlurlHttpException e)
        {
            switch (e.StatusCode)
            {
                case (int)HttpStatusCode.NotFound:
                    throw new MissingInputException(
                        $"Input for {puzzle} doesn't exist online!", e);

                case (int)HttpStatusCode.InternalServerError:
                    throw new InvalidSessionException(
                        "Your session cookie is probably invalid, update it.", e);

                default:
                    throw;
            }
        }
    }
}
