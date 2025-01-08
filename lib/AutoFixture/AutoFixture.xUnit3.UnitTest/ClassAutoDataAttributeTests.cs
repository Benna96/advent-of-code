using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Kernel;
using AutoFixture.Xunit3.UnitTest.TestTypes;
using TestTypeFoundation;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace AutoFixture.Xunit3.UnitTest
{
    public class ClassAutoDataAttributeTests
    {
        [Fact]
        public void CanCreateInstance()
        {
            // Act & Assert
            _ = new ClassAutoDataAttribute(typeof(MixedTypeClassData));
        }

        [Fact]
        public void IsDataAttribute()
        {
            // Arrange & Act
            var sut = new ClassAutoDataAttribute(typeof(MixedTypeClassData));

            // Assert
            Assert.IsAssignableFrom<DataAttribute>(sut);
        }

        [Fact]
        public void ThrowsWhenSourceTypeIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ClassAutoDataAttribute(null));
        }

        [Fact]
        public void ThrowsWhenParametersIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ClassAutoDataAttribute(typeof(MixedTypeClassData), null));
        }

        [Fact]
        public void ThrowsWhenFixtureFactoryIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DerivedClassAutoDataAttribute(null, typeof(MixedTypeClassData)));
        }

        [Fact]
        public async ValueTask GetDataThrowsWhenSourceTypeNotEnumerable()
        {
            // Arrange
            var sut = new ClassAutoDataAttribute(typeof(MyClass));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                _ = await sut.GetData(testMethod, disposalTracker));
        }

        [Fact]
        public async ValueTask GetDataThrowsWhenParametersDoNotMatchConstructor()
        {
            // Arrange
            var sut = new ClassAutoDataAttribute(typeof(MyClass), "myString", 33, null);
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            // Act & Assert
            await Assert.ThrowsAsync<MissingMethodException>(async () =>
                _ = await sut.GetData(testMethod, disposalTracker));
        }

        [Fact]
        public async ValueTask GetDataDoesNotThrowWhenSourceYieldsNoResults()
        {
            // Arrange
            var sut = new ClassAutoDataAttribute(typeof(EmptyClassData));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod))!;
            var disposalTracker = new DisposalTracker();

            // Act
            var data = await sut.GetData(testMethod, disposalTracker);

            // Assert
            Assert.Empty(data);
        }

        [Fact]
        public async ValueTask GetDataThrowsWhenSourceYieldsNullResults()
        {
            // Arrange
            var sut = new ClassAutoDataAttribute(typeof(ClassWithNullTestData));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            // Act & assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await sut.GetData(testMethod, disposalTracker));
        }

        [Fact]
        public async ValueTask GetDataDoesNotThrow()
        {
            // Arrange
            var sut = new ClassAutoDataAttribute(typeof(MixedTypeClassData));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            // Act & Assert
            _ = await sut.GetData(testMethod, disposalTracker);
        }

        [Fact]
        public async ValueTask GetDataReturnsEnumerable()
        {
            // Arrange
            var sut = new ClassAutoDataAttribute(typeof(MixedTypeClassData));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            // Act
            var actual = await sut.GetData(testMethod, disposalTracker);

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public async ValueTask GetDataReturnsNonEmptyEnumerable()
        {
            // Arrange
            var sut = new ClassAutoDataAttribute(typeof(MixedTypeClassData));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            // Act
            var actual = await sut.GetData(testMethod, disposalTracker);

            // Assert
            Assert.NotEmpty(actual);
        }

        [Fact]
        public async ValueTask GetDataReturnsExpectedTestDataCount()
        {
            // Arrange
            var sut = new ClassAutoDataAttribute(typeof(MixedTypeClassData));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            // Act
            var actual = await sut.GetData(testMethod, disposalTracker);

            // Assert
            Assert.Equal(5, actual.Count());
        }

        [Fact]
        public async ValueTask GetDataThrowsWhenDataSourceNotEnumerable()
        {
            // Arrange
            var sut = new ClassAutoDataAttribute(typeof(GuardedConstructorHost<object>));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();


            // Act & Assert
            await Assert.ThrowsAsync<MissingMethodException>(async () =>
                _ = await sut.GetData(testMethod, disposalTracker));
        }

        [Fact]
        public async ValueTask GetDataThrowsForNonMatchingConstructorTypes()
        {
            // Arrange
            var sut = new ClassAutoDataAttribute(typeof(DelegatingTestData), "myString", 33, null);
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            // Act & Assert
            await Assert.ThrowsAsync<MissingMethodException>(async () =>
                _ = await sut.GetData(testMethod, disposalTracker));
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

            var sut = new DerivedClassAutoDataAttribute(() => fixture, typeof(ClassWithEmptyTestData));

            // Act
            _ = await sut.GetData(method, disposalTracker);

            // Assert
            var composite = Assert.IsAssignableFrom<CompositeCustomization>(customizationLog[0]);
            Assert.IsNotType<FreezeOnMatchCustomization>(composite.Customizations.First());
            Assert.IsType<FreezeOnMatchCustomization>(composite.Customizations.Last());
        }

        [Fact]
        public async ValueTask GetDataReturnsExpectedTestData()
        {
            var builder = new CompositeSpecimenBuilder(
                new FixedParameterBuilder<int>("a", 1),
                new FixedParameterBuilder<string>("b", "value"),
                new FixedParameterBuilder<EnumType>("c", EnumType.First),
                new FixedParameterBuilder<Tuple<string, int>>("d", new Tuple<string, int>("value", 1)));
            var sut = new DerivedClassAutoDataAttribute(
                () => new DelegatingFixture { OnCreate = (r, c) => builder.Create(r, c) },
                typeof(MixedTypeClassData));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();
            object[][] expected =
            {
                new object[] { 1, "value", EnumType.First, new Tuple<string, int>("value", 1) },
                new object[] { 9, "value", EnumType.First, new Tuple<string, int>("value", 1) },
                new object[] { 12, "test-12", EnumType.First, new Tuple<string, int>("value", 1) },
                new object[] { 223, "test-17", EnumType.Third, new Tuple<string, int>("value", 1) },
                new object[] { -95, "test-92", EnumType.Second, new Tuple<string, int>("myValue", 5) }
            };

            var actual = await sut.GetData(testMethod, disposalTracker);

            Assert.Equal(expected, actual.Select(row => row.GetData()));
        }

        [Fact]
        public async ValueTask GetDataReturnsExpectedTestDataFromParameterizedSource()
        {
            var builder = new CompositeSpecimenBuilder(
                new FixedParameterBuilder<int>("a", 1),
                new FixedParameterBuilder<string>("b", "value"),
                new FixedParameterBuilder<Tuple<string, int>>("d", new Tuple<string, int>("value", 1)));
            var sut = new DerivedClassAutoDataAttribute(
                () => new DelegatingFixture { OnCreate = (r, c) => builder.Create(r, c) },
                typeof(ParameterizedClassData),
                29, "myValue", EnumType.Third);
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();
            object[][] expected =
            {
                new object[] { 29, "myValue", EnumType.Third, new Tuple<string, int>("value", 1) },
                new object[] { 29, "myValue", EnumType.Third, new Tuple<string, int>("value", 1) }
            };

            var actual = await sut.GetData(testMethod, disposalTracker);

            Assert.Equal(expected, actual.Select(row => row.GetData()));
        }
    }
}
