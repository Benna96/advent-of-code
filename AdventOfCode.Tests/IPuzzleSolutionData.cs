using System.Collections.Generic;

namespace AdventOfCode.Tests;

public interface IPuzzleSolutionData
{
    public static abstract IEnumerable<(string input, string solution)> Part1Data { get; }
    public static abstract IEnumerable<(string input, string solution)> Part2Data { get; }
}
