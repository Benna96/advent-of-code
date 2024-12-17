namespace AdventOfCode.Tests;

public class SampleStringTests
{
    [Fact]
    public void Advent_ne_Code()
    {
        "Advent".Should().NotBe("Code");
    }

    [Fact]
    public void AdventOfCode2024_contains_AdventOfCode()
    {
        "AdventOfCode2024".Should().Contain("AdventOfCode");
    }

    [Fact]
    public void AdventOfCode2024_doesnt_contain_AdventOfCode2023()
    {
        "AdventOfCode2024".Should().NotContain("AdventOfCode2023");
    }
}
