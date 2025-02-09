using System.Collections.Generic;
using System.Linq;

using StringReader = System.IO.StringReader;

namespace AdventOfCode;

public class Day02Solver : IPuzzleSolver
{
    public static PuzzleMetadata LinkedPuzzle { get; } = new(Day: 2);

    public static string SolvePart1(string input)
    {
        var levelReports = ReadLevelReportsFrom(input);
        var safeReports = levelReports.Where(IsSafe);

        return safeReports.Count().ToString();
    }

    public static string SolvePart2(string input)
    {
        var levelReports = ReadLevelReportsFrom(input);
        var safeReports = levelReports.Where(levels =>
        {
            if (IsSafe(levels))
                return true;

            for (var i = 0; i < levels.Count; ++i)
            {
                var levelsExceptCurrent = levels.ToList();
                levelsExceptCurrent.RemoveAt(i);
                if (IsSafe(levelsExceptCurrent))
                    return true;
            }

            return false;
        });

        return safeReports.Count().ToString();
    }

    private static bool IsSafe(IList<int> levels)
    {
        var previousLevels = levels.SkipLast(1);
        var nextLevels = levels.Skip(1);

        var increases = nextLevels.Zip(previousLevels, (next, prev) => next - prev).ToArray();

        if (increases.Any(x => Math.Abs(x) is < 1 or > 3))
            return false;

        if (increases.Select(Math.Sign).Distinct().Skip(1).Any())
            return false;

        return true;
    }

    private static IList<IList<int>> ReadLevelReportsFrom(string input)
    {
        IList<IList<int>> readLevelReports = [];

        using (var reader = new StringReader(input))
        {
            while (reader.ReadLine() is { } line)
            {
                var split = line.Split(' ');
                var levels = split.Select(int.Parse);
                readLevelReports.Add(levels.ToList());
            }
        }

        return readLevelReports;
    }
}
