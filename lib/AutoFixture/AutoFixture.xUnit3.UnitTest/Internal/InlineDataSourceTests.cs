using System;
using AutoFixture.Xunit3.Internal;
using AutoFixture.Xunit3.UnitTest.TestTypes;
using Xunit;
using Xunit.Sdk;

namespace AutoFixture.Xunit3.UnitTest.Internal;

public class InlineDataSourceTests
{
    [Fact]
    public void SutIsTestDataSource()
    {
        // Arrange
        // Act
        var sut = new InlineDataSource(Array.Empty<object>());
        // Assert
        Assert.IsAssignableFrom<IDataSource>(sut);
    }

    [Fact]
    public void InitializeWithNullValuesThrows()
    {
        // Arrange
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new InlineDataSource(null));
    }

    [Fact]
    public void ValuesIsCorrect()
    {
        // Arrange
        var expectedValues = Array.Empty<object>();
        var sut = new InlineDataSource(expectedValues);
        // Act
        var result = sut.Values;
        // Assert
        Assert.Equal(expectedValues, result);
    }

    [Fact]
    public void GetTestDataWithNullMethodThrows()
    {
        // Arrange
        var sut = new InlineDataSource(Array.Empty<object>());
        var disposalTracker = new DisposalTracker();
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            sut.GetData(null, disposalTracker));
    }

    [Fact]
    public void SourceThrowsWhenArgumentCountExceedParameterCount()
    {
        // Arrange
        var values = new object[] { "aloha", 42, 12.3d, "extra" };
        var sut = new InlineDataSource(values);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));
        var disposalTracker = new DisposalTracker();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            sut.GetData(testMethod, disposalTracker));
    }

    [Fact]
    public void ReturnsTestDataWhenArgumentCountMatchesParameterCount()
    {
        // Arrange
        var values = new object[] { "aloha", 42, 12.3d };
        var sut = new InlineDataSource(values);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));
        var disposalTracker = new DisposalTracker();

        // Act
        var result = sut.GetData(testMethod, disposalTracker);

        // Assert
        var testData = Assert.Single(result);
        Assert.Equal(values, testData);
    }

    [Fact]
    public void ReturnsAllArgumentsWhenArgumentCountLessThanParameterCount()
    {
        // Arrange
        var values = new object[] { "aloha", 42 };
        var sut = new InlineDataSource(values);
        var testMethod = typeof(SampleTestType)
            .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));
        var disposalTracker = new DisposalTracker();

        // Act
        var result = sut.GetData(testMethod, disposalTracker);

        // Assert
        var testData = Assert.Single(result);
        Assert.Equal(values, testData);
    }
}