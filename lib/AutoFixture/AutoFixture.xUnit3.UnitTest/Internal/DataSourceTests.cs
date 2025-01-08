using System;
using System.Linq;
using AutoFixture.Xunit3.Internal;
using AutoFixture.Xunit3.UnitTest.TestTypes;
using Xunit;
using Xunit.Sdk;

namespace AutoFixture.Xunit3.UnitTest.Internal
{
    public class DataSourceTests
    {
        [Fact]
        public void SutIsTestDataSource()
        {
            // Arrange
            var sut = new DelegatingDataSource();

            // Assert
            Assert.IsAssignableFrom<IDataSource>(sut);
        }

        [Fact]
        public void ThrowsWhenInvokedWithNullMethodInfo()
        {
            // Arrange
            var disposalTracker = new DisposalTracker();
            var sut = new DelegatingDataSource();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => sut.GetData(null, disposalTracker));
        }

        [Fact]
        public void ReturnSingleEmptyArrayWhenMethodHasNoParameters()
        {
            // Arrange
            var sut = new DelegatingDataSource();
            var testMethod = typeof(SampleTestType)
                .GetMethod(nameof(SampleTestType.TestMethodWithoutParameters));
            var disposalTracker = new DisposalTracker();

            // Act
            var result = sut.GetData(testMethod, disposalTracker).ToArray();

            // Assert
            var item = Assert.Single(result);
            Assert.Empty(item);
        }

        [Fact]
        public void ThrowsWhenNoDataFoundForMethod()
        {
            // Arrange
            var sut = new DelegatingDataSource { TestData = null };
            var testMethod = typeof(SampleTestType)
                .GetMethod(nameof(SampleTestType.TestMethodWithSingleParameter));
            var disposalTracker = new DisposalTracker();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => sut.GetData(testMethod, disposalTracker).ToArray());
        }

        [Fact]
        public void ReturnSingleArrayWithSingleItemWhenMethodHasSingleParameter()
        {
            // Arrange
            var sut = new DelegatingDataSource
            {
                TestData = new[] { new object[] { "hello" } }
            };
            var testMethod = typeof(SampleTestType)
                .GetMethod(nameof(SampleTestType.TestMethodWithSingleParameter));
            var disposalTracker = new DisposalTracker();

            // Act
            var result = sut.GetData(testMethod, disposalTracker).ToArray();

            // Assert
            var testData = Assert.Single(result);
            var argument = Assert.Single(testData);
            Assert.Equal("hello", argument);
        }

        [Fact]
        public void ReturnsArgumentsFittingTestParameters()
        {
            // Arrange
            var testData = new[]
            {
                new object[] { "hello", 16, 32.86d },
                new object[] { null, -1, -20.22 },
                new object[] { "one", 2 },
                new object[] { null },
                new object[] { },
            };
            var sut = new DelegatingDataSource { TestData = testData };
            var testMethod = typeof(SampleTestType)
                .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));
            var disposalTracker = new DisposalTracker();

            // Act
            var actual = sut.GetData(testMethod, disposalTracker).ToArray();

            // Assert
            Assert.Equal(testData.Length, actual.Length);
            Assert.All(actual, x => Assert.InRange(x.Length, 0, 3));
        }

        [Fact]
        public void ThrowsWhenTestDataContainsMoreArgumentsThanParameters()
        {
            // Arrange
            var testData = new[] { new object[] { "hello", 16, 32.86d, "extra" } };
            var sut = new DelegatingDataSource { TestData = testData };
            var testMethod = typeof(SampleTestType)
                .GetMethod(nameof(SampleTestType.TestMethodWithMultipleParameters));
            var disposalTracker = new DisposalTracker();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => sut.GetData(testMethod, disposalTracker).ToArray());
        }
    }
}