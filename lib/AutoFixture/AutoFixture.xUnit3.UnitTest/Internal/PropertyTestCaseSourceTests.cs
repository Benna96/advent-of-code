using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoFixture.Xunit3.Internal;
using TestTypeFoundation;
using Xunit;
using Xunit.Sdk;

namespace AutoFixture.Xunit3.UnitTest.Internal
{
    public class PropertyTestCaseSourceTests
    {
        [Fact]
        public void SutIsTestCaseSource()
        {
            // Arrange
            var sourceProperty = typeof(PropertyTestCaseSourceTests)
                .GetProperty(nameof(TestDataPropertyWithMixedValues));
            var sut = new PropertyTestCaseSource(sourceProperty);

            // Assert
            Assert.IsAssignableFrom<ITestCaseSource>(sut);
        }

        public static IEnumerable<object[]> TestDataPropertyWithMixedValues => new[]
        {
            new object[] { "hello", 1, new PropertyHolder<string> { Property = "world" } },
            new object[] { "foo", 2, new PropertyHolder<string> { Property = "bar" } },
            new object[] { "Han", 3, new PropertyHolder<string> { Property = "Solo" } }
        };

        public static object NonEnumerableProperty => new object();

        [Fact]
        public void ThrowsWhenConstructedWithNullProperty()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new PropertyTestCaseSource(null!));
        }

        [Fact]
        public void PropertyIsCorrect()
        {
            // Arrange
            var expected = typeof(PropertyTestCaseSourceTests)
                .GetProperty(nameof(TestDataPropertyWithMixedValues));
            var sut = new PropertyTestCaseSource(expected);

            // Act
            var result = sut.PropertyInfo;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ThrowsWhenInvokedWithNullTestMethod()
        {
            // Arrange
            var sourceProperty = typeof(PropertyTestCaseSourceTests)
                .GetProperty(nameof(TestDataPropertyWithMixedValues));
            var sut = new PropertyTestCaseSource(sourceProperty);
            var disposalTracker = new DisposalTracker();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => sut.GetTestCases(null!, disposalTracker));
        }

        [Fact]
        public void ThrowsWhenSourceIsNotEnumerable()
        {
            // Arrange
            var sourceProperty = typeof(PropertyTestCaseSourceTests)
                .GetProperty(nameof(NonEnumerableProperty));
            var sut = new PropertyTestCaseSource(sourceProperty);
            var method = typeof(SampleTestType)
                .GetMethod(nameof(SampleTestType.TestMethodWithReferenceTypeParameter));
            var disposalTracker = new DisposalTracker();

            // Act & Assert
            Assert.Throws<InvalidCastException>(
                () => sut.GetTestCases(method, disposalTracker).ToArray());
        }

        [Fact]
        public void GeneratesTestDataMatchingTestParameters()
        {
            // Arrange
            var expected = new[]
            {
                new object[] { "hello", 1, new RecordType<string>("world") },
                new object[] { "foo", 2, new RecordType<string>("bar") },
                new object[] { "Han", 3, new RecordType<string>("Solo") }
            };
            var sourceProperty = typeof(PropertyTestCaseSourceTests)
                .GetProperty(nameof(TestDataPropertyWithRecordValues));
            var sut = new PropertyTestCaseSource(sourceProperty);
            var method = typeof(SampleTestType)
                .GetMethod(nameof(SampleTestType.TestMethodWithRecordTypeParameter));
            var disposalTracker = new DisposalTracker();

            // Act
            var result = sut.GetTestCases(method, disposalTracker).ToArray();

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> TestDataPropertyWithRecordValues => new[]
        {
            new object[] { "hello", 1, new RecordType<string>("world") },
            new object[] { "foo", 2, new RecordType<string>("bar") },
            new object[] { "Han", 3, new RecordType<string>("Solo") }
        };

        [Fact]
        public void ReturnsNullArguments()
        {
            // Arrange
            var expected = new[]
            {
                new object[] { null, 1, null },
                new object[] { null, 2, null },
                new object[] { null, 3, null }
            };
            var sourceProperty = typeof(PropertyTestCaseSourceTests)
                .GetProperty(nameof(TestDataPropertyWithNullValues));
            var sut = new PropertyTestCaseSource(sourceProperty);
            var testMethod = typeof(SampleTestType)
                .GetMethod(nameof(SampleTestType.TestMethodWithRecordTypeParameter));
            var disposalTracker = new DisposalTracker();

            // Act
            var result = sut.GetTestCases(testMethod, disposalTracker).ToArray();

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> TestDataPropertyWithNullValues => new[]
        {
            new object[] { null, 1, null },
            new object[] { null, 2, null },
            new object[] { null, 3, null }
        };
    }
}