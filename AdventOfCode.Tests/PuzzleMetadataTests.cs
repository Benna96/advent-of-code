namespace AdventOfCode.Tests;

public class PuzzleMetadataTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(26)]
    public void Rejects_nonadvent_days(int day)
    {
        var creator = () => _ = new PuzzleMetadata(Day: day);
        creator.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    public void Accepts_advent_days(int day)
    {
        var creator = () => _ = new PuzzleMetadata(Day: day);
        creator.Should().NotThrow();
    }
}
