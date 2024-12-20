using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode;

public class Day01Solver : IPuzzleSolver
{
    public static PuzzleMetadata LinkedPuzzle { get; } = new(Day: 1);

    public static string SolvePart1(string input)
    {
        List<int> leftIds = [];
        List<int> rightIds = [];

        using (var reader = new StringReader(input))
        {
            while (reader.ReadLine() is { } line)
            {
                var split = line.Split("   ");
                leftIds.Add(int.Parse(split[0]));
                rightIds.Add(int.Parse(split[1]));
            }
        }

        leftIds.Sort();
        rightIds.Sort();

        var pairs = leftIds.Zip(rightIds, (left, right) => (left, right));
        var distanceSum = pairs.Sum(pair => Math.Abs(pair.left - pair.right));

        return distanceSum.ToString();
    }

    public static string SolvePart2(string input)
    {
        List<int> leftIds = [];
        List<int> rightIds = [];

        using (var reader = new StringReader(input))
        {
            while (reader.ReadLine() is { } line)
            {
                var split = line.Split("   ");
                leftIds.Add(int.Parse(split[0]));
                rightIds.Add(int.Parse(split[1]));
            }
        }

        Dictionary<int, int> rightIdCounts = [];
        foreach (var id in rightIds)
        {
            var idCount = rightIdCounts.GetValueOrDefault(id, 0);
            ++idCount;
            rightIdCounts[id] = idCount;
        }

        var similarityScore = leftIds.Sum(id => id * rightIdCounts.GetValueOrDefault(id, 0));
        return similarityScore.ToString();
    }
}
