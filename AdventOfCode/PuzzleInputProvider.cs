using System.IO;

namespace AdventOfCode;

public static class PuzzleInputProvider
{
    public static readonly string InputFolder = "Inputs";

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
            throw new MissingInputException($"Input for {puzzle} is missing!");
        }

        return File.ReadAllText(path);
    }
}
