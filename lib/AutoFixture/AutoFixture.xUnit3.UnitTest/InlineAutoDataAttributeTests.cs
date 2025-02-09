﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.Xunit3.UnitTest.TestTypes;
using TestTypeFoundation;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace AutoFixture.Xunit3.UnitTest
{
    public class InlineAutoDataAttributeTests
    {
        [Fact]
        public void SutIsDataAttribute()
        {
            // Arrange & Act
            var sut = new InlineAutoDataAttribute();

            // Assert
            Assert.IsAssignableFrom<DataAttribute>(sut);
        }

        [Fact]
        public void ValuesWillBeEmptyWhenSutIsCreatedWithDefaultConstructor()
        {
            // Arrange
            var sut = new InlineAutoDataAttribute();
            var expected = Enumerable.Empty<object>();

            // Act
            var result = sut.Values;

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ValuesWillNotBeEmptyWhenSutIsCreatedWithConstructorArguments()
        {
            // Arrange
            var expectedValues = new[] { new object(), new object(), new object() };
            var sut = new InlineAutoDataAttribute(expectedValues);

            // Act
            var result = sut.Values;

            // Assert
            Assert.True(result.SequenceEqual(expectedValues));
        }

        [Fact]
        public void ValuesAreCorrectWhenConstructedWithExplicitAutoDataAttribute()
        {
            // Arrange
            var expectedValues = new[] { new object(), new object(), new object() };
            var sut = new DerivedInlineAutoDataAttribute(() => new DelegatingFixture(), expectedValues);

            // Act
            var result = sut.Values;

            // Assert
            Assert.Equal(expectedValues, result);
        }

        [Fact]
        public void DoesntActivateFixtureImmediately()
        {
            // Arrange
            var wasInvoked = false;
            Func<IFixture> autoData = () =>
            {
                wasInvoked = true;
                return new DelegatingFixture();
            };

            // Act
            _ = new DerivedInlineAutoDataAttribute(autoData);

            // Assert
            Assert.False(wasInvoked);
        }

        [Fact]
        public void PreDiscoveryShouldBeDisabled()
        {
            // Arrange
            var sut = new AutoDataAttribute();

            // Act & assert
            Assert.False(sut.SupportsDiscoveryEnumeration());
        }

        [Theory]
        [InlineData("CreateWithFrozenAndFavorArrays")]
        [InlineData("CreateWithFavorArraysAndFrozen")]
        [InlineData("CreateWithFrozenAndFavorEnumerables")]
        [InlineData("CreateWithFavorEnumerablesAndFrozen")]
        [InlineData("CreateWithFrozenAndFavorLists")]
        [InlineData("CreateWithFavorListsAndFrozen")]
        [InlineData("CreateWithFrozenAndGreedy")]
        [InlineData("CreateWithGreedyAndFrozen")]
        [InlineData("CreateWithFrozenAndModest")]
        [InlineData("CreateWithModestAndFrozen")]
        [InlineData("CreateWithFrozenAndNoAutoProperties")]
        [InlineData("CreateWithNoAutoPropertiesAndFrozen")]
        public async ValueTask GetDataOrdersCustomizationAttributes(string methodName)
        {
            // Arrange
            var method = typeof(TypeWithCustomizationAttributes)
                .GetMethod(methodName, new[] { typeof(ConcreteType) });
            var disposalTracker = new DisposalTracker();
            var customizationLog = new List<ICustomization>();
            var fixture = new DelegatingFixture
            {
                OnCustomize = c => customizationLog.Add(c)
            };
            var sut = new DerivedInlineAutoDataAttribute(() => fixture);

            // Act
            _ = await sut.GetData(method, disposalTracker);

            // Assert
            var composite = Assert.IsAssignableFrom<CompositeCustomization>(customizationLog[0]);
            Assert.IsNotType<FreezeOnMatchCustomization>(composite.Customizations.First());
            Assert.IsType<FreezeOnMatchCustomization>(composite.Customizations.Last());
        }

        [Theory]
        [ClassData(typeof(InlinePrimitiveValuesTestData))]
        [ClassData(typeof(InlineFrozenValuesTestData))]
        public async ValueTask ReturnsSingleTestDataWithExpectedValues(
            DataAttribute attribute, MethodInfo testMethod, object[] expected)
        {
            // Arrange
            var disposalTracker = new DisposalTracker();

            // Act
            var actual = await attribute.GetData(testMethod, disposalTracker);

            // Assert
            Assert.Single(actual);
            Assert.Equal(expected, actual.ElementAt(0).GetData());
        }

        [Theory]
        [InlineAutoData]
        public void GeneratesRandomData(int a, float b, string c, decimal d)
        {
            Assert.NotEqual(0, a);
            Assert.NotEqual(0, b);
            Assert.NotNull(c);
            Assert.NotEqual(0, d);
        }

        [Theory]
        [InlineAutoData(12, 32.1f, "hello", 71.231d)]
        public void InlinesAllData(int a, float b, string c, decimal d)
        {
            Assert.Equal(12, a);
            Assert.Equal(32.1f, b);
            Assert.Equal("hello", c);
            Assert.Equal(71.231m, d);
        }

        [Theory]
        [InlineAutoData(0)]
        [InlineAutoData(5)]
        [InlineAutoData(-12)]
        [InlineAutoData(21.3f)]
        [InlineAutoData(18.7d)]
        [InlineAutoData(EnumType.First)]
        [InlineAutoData("Hello World")]
        [InlineAutoData("\t\r\n")]
        [InlineAutoData(" ")]
        [InlineAutoData("")]
        [InlineAutoData((object)null)]
        public void InjectsInlineValues(
            [Frozen] object a,
            [Frozen] PropertyHolder<object> value,
            PropertyHolder<object> frozen)
        {
            Assert.Equal(a, value.Property);
            Assert.Same(frozen, value);
        }
    }
}
