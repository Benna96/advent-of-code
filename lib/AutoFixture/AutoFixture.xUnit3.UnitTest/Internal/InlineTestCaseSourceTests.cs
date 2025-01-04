using System;
using System.Reflection;
using AutoFixture.Xunit3.Internal;
using Xunit;
using Xunit.Sdk;

namespace AutoFixture.Xunit3.UnitTest.Internal;

public class InlineTestCaseSourceTests
{
    [Fact]
    public void SutIsTestCaseSource()
    {
        // Arrange
        // Act
        var sut = new InlineTestCaseSource(Array.Empty<object>());
        // Assert
        Assert.IsAssignableFrom<ITestCaseSource>(sut);
    }

    [Fact]
    public void InitializeWithNullValuesThrows()
    {
        // Arrange
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new InlineTestCaseSource(null));
    }

    [Fact]
    public void ValuesIsCorrect()
    {
        // Arrange
        var expectedValues = Array.Empty<object>();
        var sut = new InlineTestCaseSource(expectedValues);
        // Act
        var result = sut.Values;
        // Assert
        Assert.Equal(expectedValues, result);
    }

    [Fact]
    public void GetTestCasesWithNullMethodThrows()
    {
        // Arrange
        var sut = new InlineTestCaseSource(Array.Empty<object>());
        var disposalTracker = new DisposalTracker();
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            sut.GetTestCases(null, disposalTracker));
    }

    [Fact]
    public void SourceThrowsWhenArgumentCountExceedParameterCount()
    {
        // Arrange
        var values = new object[] { "aloha", 42, 12.3d, "extra" };
        var sut = new InlineTestCaseSource(values);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));
        var disposalTracker = new DisposalTracker();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            sut.GetTestCases(testMethod, disposalTracker));
    }

    [Fact]
    public void ReturnsTestCaseWhenArgumentCountMatchesParameterCount()
    {
        // Arrange
        var values = new object[] { "aloha", 42, 12.3d };
        var sut = new InlineTestCaseSource(values);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));
        var disposalTracker = new DisposalTracker();

        // Act
        var result = sut.GetTestCases(testMethod, disposalTracker);

        // Assert
        var testCase = Assert.Single(result);
        Assert.Equal(values, testCase);
    }

    [Fact]
    public void ReturnsAllArgumentsWhenArgumentCountLessThanParameterCount()
    {
        // Arrange
        var values = new object[] { "aloha", 42 };
        var sut = new InlineTestCaseSource(values);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));
        var disposalTracker = new DisposalTracker();

        // Act
        var result = sut.GetTestCases(testMethod, disposalTracker);

        // Assert
        var testCase = Assert.Single(result);
        Assert.Equal(values, testCase);
    }
}