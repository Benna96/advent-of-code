namespace AdventOfCode.Tests;

public class SampleFormatterTests
{
    [Fact]
    public void Doesnt_format_two_years_as_the_same()
    {
        var first = SampleFormatter.Format(2024);
        var second = SampleFormatter.Format(2023);
        first.Should().NotBe(second);
    }

    [Fact]
    public void Modifies_format_in_some_way()
    {
        var formatted = SampleFormatter.Format(2024);
        formatted.Should().NotBe(2024.ToString());
    }

    [Theory]
    [InlineData(-2024)]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(int.MinValue)]
    public void Doesnt_allow_nonpositive_years(int nonPositiveYear)
    {
        var formatAction = () => SampleFormatter.Format(nonPositiveYear);
        formatAction.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2024)]
    [InlineData(int.MaxValue)]
    public void Allows_positive_years(int positiveYear)
    {
        var formatAction = () => SampleFormatter.Format(positiveYear);
        formatAction.Should().NotThrow<ArgumentOutOfRangeException>();
    }
}
