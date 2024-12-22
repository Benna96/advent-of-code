using Flurl;
using Flurl.Http;
using System.IO;
using System.Net;

namespace AdventOfCode;

public static class PuzzleInputProvider
{
    public static readonly string InputFolder = "Inputs";
    public static readonly string SessionOptionName = "AOC_SESSION";

    private static string GetPathForPuzzle(PuzzleMetadata puzzle)
    {
        var zeroPadded = $"{puzzle.Day:00}";
        var path = Path.Join(InputFolder, $"{zeroPadded}.txt");
        return path;
    }

    public static string GetInputForPuzzle(PuzzleMetadata puzzle)
    {
        var path = GetPathForPuzzle(puzzle);
        if (!File.Exists(path))
        {
            DownloadPuzzleInputTo(puzzle, path);
        }

        return File.ReadAllText(path);
    }

    private static void DownloadPuzzleInputTo(PuzzleMetadata puzzle, string path)
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
            var folderPath = Path.GetDirectoryName(path);
            var fileName = Path.GetFileName(path);
            request.DownloadFileAsync(folderPath, fileName)
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
