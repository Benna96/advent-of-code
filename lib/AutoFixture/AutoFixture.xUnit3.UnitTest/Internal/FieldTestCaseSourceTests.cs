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
    public class FieldTestCaseSourceTests
    {
        public static IEnumerable<object[]> TestDataFieldWithMixedValues = new[]
        {
            new object[] { "hello", 1, new FieldHolder<string> { Field = "world" } },
            new object[] { "foo", 2, new FieldHolder<string> { Field = "bar" } },
            new object[] { "Han", 3, new FieldHolder<string> { Field = "Solo" } }
        };

        public static object NonEnumerableField = new object();

        [Fact]
        public void SutIsTestCaseSource()
        {
            // Arrange
            var sourceField = typeof(FieldTestCaseSourceTests)
                .GetField(nameof(TestDataFieldWithMixedValues));
            var sut = new FieldTestCaseSource(sourceField);

            // Assert
            Assert.IsAssignableFrom<ITestCaseSource>(sut);
        }

        [Fact]
        public void ThrowsWhenConstructedWithNullField()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new FieldTestCaseSource(null!));
        }

        [Fact]
        public void FieldIsCorrect()
        {
            // Arrange
            var expected = typeof(FieldTestCaseSourceTests)
                .GetField(nameof(TestDataFieldWithMixedValues));
            var sut = new FieldTestCaseSource(expected);

            // Act
            var result = sut.FieldInfo;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ThrowsWhenInvokedWithNullTestMethod()
        {
            // Arrange
            var sourceField = typeof(FieldTestCaseSourceTests)
                .GetField(nameof(TestDataFieldWithMixedValues));
            var sut = new FieldTestCaseSource(sourceField);
            var disposalTracker = new DisposalTracker();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => sut.GetTestCases(null!, disposalTracker));
        }

        [Fact]
        public void ThrowsWhenSourceIsNotEnumerable()
        {
            // Arrange
            var sourceField = typeof(FieldTestCaseSourceTests)
                .GetField(nameof(NonEnumerableField));
            var sut = new FieldTestCaseSource(sourceField);
            var method = typeof(SampleTestType)
                .GetMethod(nameof(SampleTestType.TestMethodWithReferenceTypeParameter));
            var disposalTracker = new DisposalTracker();

            // Act & Assert
            Assert.Throws<InvalidCastException>(() => sut.GetTestCases(method, disposalTracker).ToArray());
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
            var sourceField = typeof(FieldTestCaseSourceTests)
                .GetField(nameof(TestDataFieldWithRecordValues));
            var sut = new FieldTestCaseSource(sourceField);
            var method = typeof(SampleTestType)
                .GetMethod(nameof(SampleTestType.TestMethodWithRecordTypeParameter));
            var disposalTracker = new DisposalTracker();

            // Act
            var result = sut.GetTestCases(method, disposalTracker).ToArray();

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> TestDataFieldWithRecordValues = new[]
        {
            new object[] { "hello", 1, new RecordType<string>("world") },
            new object[] { "foo", 2, new RecordType<string>("bar") },
            new object[] { "Han", 3, new RecordType<string>("Solo") }
        };
    }
}