using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode;

public class Day02Solver
{
    public static string SolvePart1(string input)
    {
        var levelReports = ReadLevelReportsFrom(input);
        var safeReports = levelReports.Where(IsSafe);

        return safeReports.Count().ToString();

        static IList<IList<int>> ReadLevelReportsFrom(string inputString)
        {
            IList<IList<int>> readLevelReports = [];

            using (var reader = new StringReader(inputString))
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

        static bool IsSafe(IList<int> levels)
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

        static IList<IList<int>> ReadLevelReportsFrom(string inputString)
        {
            IList<IList<int>> readLevelReports = [];

            using (var reader = new StringReader(inputString))
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

        static bool IsSafe(IList<int> levels)
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
    }
}
