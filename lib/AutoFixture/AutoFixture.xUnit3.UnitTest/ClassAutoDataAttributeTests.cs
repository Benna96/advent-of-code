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
            _ = new ClassAutoDataAttribute(typeof(MixedTypeClassData));
        }

        [Fact]
        public void IsDataAttribute()
        {
            var sut = new ClassAutoDataAttribute(typeof(MixedTypeClassData));

            Assert.IsAssignableFrom<DataAttribute>(sut);
        }

        [Fact]
        public void ThrowsWhenSourceTypeIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ClassAutoDataAttribute(null));
        }

        [Fact]
        public void ThrowsWhenParametersIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ClassAutoDataAttribute(typeof(MixedTypeClassData), null));
        }

        [Fact]
        public void ThrowsWhenFixtureFactoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DerivedClassAutoDataAttribute(null, typeof(MixedTypeClassData)));
        }

        [Fact]
        public async ValueTask GetDataThrowsWhenSourceTypeNotEnumerable()
        {
            var sut = new ClassAutoDataAttribute(typeof(MyClass));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            Func<Task> act = async () =>
                _ = await sut.GetData(testMethod, disposalTracker);

            await Assert.ThrowsAsync<InvalidOperationException>(act);
        }

        [Fact]
        public async ValueTask GetDataThrowsWhenParametersDoNotMatchConstructor()
        {
            var sut = new ClassAutoDataAttribute(typeof(MyClass), "myString", 33, null);
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            Func<Task> act = async () =>
                _ = await sut.GetData(testMethod, disposalTracker);

            await Assert.ThrowsAsync<MissingMethodException>(act);
        }

        [Fact]
        public async ValueTask GetDataDoesNotThrowWhenSourceYieldsNoResults()
        {
            var sut = new ClassAutoDataAttribute(typeof(EmptyClassData));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod))!;
            var disposalTracker = new DisposalTracker();

            var data = await sut.GetData(testMethod, disposalTracker);

            Assert.Empty(data);
        }

        [Fact]
        public async ValueTask GetDataThrowsWhenSourceYieldsNullResults()
        {
            // Arrange
            var sut = new ClassAutoDataAttribute(typeof(ClassWithNullTestCases));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            // Act & assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await sut.GetData(testMethod, disposalTracker));
        }

        [Fact]
        public async ValueTask GetDataDoesNotThrow()
        {
            var sut = new ClassAutoDataAttribute(typeof(MixedTypeClassData));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            _ = await sut.GetData(testMethod, disposalTracker);
        }

        [Fact]
        public async ValueTask GetDataReturnsEnumerable()
        {
            var sut = new ClassAutoDataAttribute(typeof(MixedTypeClassData));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            var actual = await sut.GetData(testMethod, disposalTracker);

            Assert.NotNull(actual);
        }

        [Fact]
        public async ValueTask GetDataReturnsNonEmptyEnumerable()
        {
            var sut = new ClassAutoDataAttribute(typeof(MixedTypeClassData));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            var actual = await sut.GetData(testMethod, disposalTracker);

            Assert.NotEmpty(actual);
        }

        [Fact]
        public async ValueTask GetDataReturnsExpectedTestCaseCount()
        {
            var sut = new ClassAutoDataAttribute(typeof(MixedTypeClassData));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            var actual = await sut.GetData(testMethod, disposalTracker);

            Assert.Equal(5, actual.Count());
        }

        [Fact]
        public async ValueTask GetDataThrowsWhenDataSourceNotEnumerable()
        {
            var sut = new ClassAutoDataAttribute(typeof(GuardedConstructorHost<object>));
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            Func<Task> act = async () =>
                _ = await sut.GetData(testMethod, disposalTracker);

            await Assert.ThrowsAsync<MissingMethodException>(act);
        }

        [Fact]
        public async ValueTask GetDataThrowsForNonMatchingConstructorTypes()
        {
            var sut = new ClassAutoDataAttribute(
                typeof(DelegatingTestData), "myString", 33, null);
            var testMethod = typeof(ExampleTestClass).GetMethod(nameof(ExampleTestClass.TestMethod));
            var disposalTracker = new DisposalTracker();

            Func<Task> act = async () =>
                _ = await sut.GetData(testMethod, disposalTracker);

            await Assert.ThrowsAsync<MissingMethodException>(act);
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

            var sut = new DerivedClassAutoDataAttribute(() => fixture, typeof(ClassWithEmptyTestCases));

            // Act
            _ = await sut.GetData(method, disposalTracker);

            // Assert
            var composite = Assert.IsAssignableFrom<CompositeCustomization>(customizationLog[0]);
            Assert.IsNotType<FreezeOnMatchCustomization>(composite.Customizations.First());
            Assert.IsType<FreezeOnMatchCustomization>(composite.Customizations.Last());
        }

        [Fact]
        public async ValueTask GetDataReturnsExpectedTestCases()
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
        public async ValueTask GetDataReturnsExpectedTestCasesFromParameterizedSource()
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
