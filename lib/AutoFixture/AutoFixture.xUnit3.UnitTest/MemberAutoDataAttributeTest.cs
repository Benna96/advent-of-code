using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local",
        Justification = "Using parameter for precondition checks is acceptable in assertions.")]
    public class MemberAutoDataAttributeTest
    {
        [Fact]
        public void SutIsDataAttribute()
        {
            // Arrange
            var memberName = Guid.NewGuid().ToString();

            // Act
            var sut = new MemberAutoDataAttribute(memberName);

            // Assert
            Assert.IsAssignableFrom<DataAttribute>(sut);
        }

        [Fact]
        public void InitializedWithMemberNameAndParameters()
        {
            // Arrange
            var memberName = Guid.NewGuid().ToString();
            var parameters = new object[] { "value-one", 3, 12.2f };

            // Act
            var sut = new MemberAutoDataAttribute(memberName, parameters);

            // Assert
            Assert.Equal(memberName, sut.MemberName);
            Assert.Equal(parameters, sut.Parameters);
            Assert.Null(sut.MemberType);
            Assert.NotNull(sut.FixtureFactory);
        }

        [Fact]
        public void InitializedWithTypeMemberNameAndParameters()
        {
            // Arrange
            var memberName = Guid.NewGuid().ToString();
            var parameters = new object[] { "value-one", 3, 12.2f };
            var testType = typeof(MemberAutoDataAttributeTest);

            // Act
            var sut = new MemberAutoDataAttribute(testType, memberName, parameters);

            // Assert
            Assert.Equal(memberName, sut.MemberName);
            Assert.Equal(parameters, sut.Parameters);
            Assert.Equal(testType, sut.MemberType);
            Assert.NotNull(sut.FixtureFactory);
        }

        [Fact]
        public void ThrowsWhenInitializedWithNullMemberName()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new MemberAutoDataAttribute(null!));
        }

        [Fact]
        public void ThrowsWhenInitializedWithNullParameters()
        {
            // Arrange
            var memberName = Guid.NewGuid().ToString();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => new MemberAutoDataAttribute(memberName, null!));
        }

        [Fact]
        public void DoesNotThrowWhenInitializedWithNullType()
        {
            // Arrange
            var memberName = Guid.NewGuid().ToString();

            // Act & Assert
            _ = new MemberAutoDataAttribute(null!, memberName);
        }

        [Fact]
        public async ValueTask ThrowsWhenTestMethodNull()
        {
            // Arrange
            var disposalTracker = new DisposalTracker();
            var sut = new MemberAutoDataAttribute("memberName");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await sut.GetData(null!, disposalTracker));
        }

        [Fact]
        public async ValueTask ThrowsWhenMemberNotEnumerable()
        {
            // Arrange
            var memberName = nameof(TestTypeWithMethodData.NonEnumerableMethod);
            var sut = new MemberAutoDataAttribute(memberName);
            var method = TestTypeWithMethodData.GetNonEnumerableMethodInfo();
            var disposalTracker = new DisposalTracker();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () =>
                await sut.GetData(method, disposalTracker));
            Assert.Contains(memberName, ex.Message);
        }

        [Fact]
        public async ValueTask ThrowsWhenMemberNotStatic()
        {
            // Arrange
            const string memberName = nameof(TestTypeWithMethodData.NonStaticSource);
            var sut = new MemberAutoDataAttribute(memberName);
            var method = TestTypeWithMethodData.GetNonStaticSourceMethodInfo();
            var disposalTracker = new DisposalTracker();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () =>
                await sut.GetData(method, disposalTracker));
            Assert.Contains(memberName, ex.Message);
        }

        [Fact]
        public async ValueTask ThrowsWhenMemberDoesNotExist()
        {
            // Arrange
            var memberName = Guid.NewGuid().ToString();
            var sut = new MemberAutoDataAttribute(typeof(TestTypeWithMethodData), memberName);
            var method = TestTypeWithMethodData.GetMultipleValueTestMethodInfo();
            var disposalTracker = new DisposalTracker();

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () =>
                await sut.GetData(method, disposalTracker));
            Assert.Contains(memberName, ex.Message);
        }

        [Fact]
        public void DoesntActivateFixtureImmediately()
        {
            // Arrange
            var memberName = Guid.NewGuid().ToString();
            var wasInvoked = false;

            // Act
            _ = new DerivedMemberAutoDataAttribute(() =>
            {
                wasInvoked = true;
                return new DelegatingFixture();
            }, memberName);

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
                .GetMethod(methodName, new[] { typeof(ConcreteType) })!;
            var disposalTracker = new DisposalTracker();
            var customizationLog = new List<ICustomization>();
            var fixture = new DelegatingFixture
            {
                OnCustomize = c => customizationLog.Add(c)
            };

            var sut = new DerivedMemberAutoDataAttribute(
                () => fixture,
                typeof(TestTypeWithMethodData),
                nameof(TestTypeWithMethodData.TestDataWithNoValues));

            // Act
            _ = await sut.GetData(method, disposalTracker);

            // Assert
            var composite = Assert.IsAssignableFrom<CompositeCustomization>(customizationLog[0]);
            Assert.IsNotType<FreezeOnMatchCustomization>(composite.Customizations.First());
            Assert.IsType<FreezeOnMatchCustomization>(composite.Customizations.Last());
        }

        [Fact]
        public async ValueTask GeneratesTestsFromParameterlessMethod()
        {
            const string memberName = nameof(TestTypeWithMethodData.GetSingleStringValueTestData);
            var sut = new MemberAutoDataAttribute(memberName);
            var testMethod = TestTypeWithMethodData.GetSingleStringValueTestMethodInfo();
            var disposalTracker = new DisposalTracker();
            var expected = new[]
            {
                new object[] { "value-one" },
                new object[] { "value-two" },
                new object[] { "value-three" }
            };

            var testData = (await sut.GetData(testMethod, disposalTracker))
                .Select(row => row.GetData())
                .ToArray();

            Assert.Equal(testData, expected);
        }

        [Fact]
        public async ValueTask GeneratesTestsFromMethodWithParameter()
        {
            const string memberName = nameof(TestTypeWithMethodData.GetStringTestsFromArgument);
            var sut = new MemberAutoDataAttribute(memberName, "value");
            var testMethod = TestTypeWithMethodData.GetStringTestsFromArgumentMethodInfo();
            var disposalTracker = new DisposalTracker();
            var expected = new[]
            {
                new object[] { "value-one" },
                new object[] { "value-two" },
                new object[] { "value-three" }
            };

            var testData = (await sut.GetData(testMethod, disposalTracker))
                .Select(row => row.GetData())
                .ToArray();

            Assert.Equal(testData, expected);
        }

        [Fact]
        public async ValueTask GeneratesTestDataForTestsWithMultipleParameters()
        {
            // Arrange
            const string memberName = nameof(TestTypeWithMethodData.GetMultipleValueTestData);
            var sut = new MemberAutoDataAttribute(memberName);
            var testMethod = TestTypeWithMethodData.GetMultipleValueTestMethodInfo();
            var disposalTracker = new DisposalTracker();
            var expected = new[]
            {
                new object[] { "value-one", 12, 23.3m },
                new object[] { "value-two", 38, 12.7m },
                new object[] { "value-three", 94, 52.21m }
            };

            // Act
            var testCases = (await sut.GetData(testMethod, disposalTracker))
                .Select(row => row.GetData())
                .ToArray();

            // Assert
            Assert.Equal(testCases, expected);
        }

        [Fact]
        public async ValueTask GeneratesMissingDataForTestsWithMultipleParameters()
        {
            // Arrange
            const string memberName = nameof(TestTypeWithMethodData.GetSingleStringValueTestData);
            var sut = new MemberAutoDataAttribute(memberName);
            var testMethod = TestTypeWithMethodData.GetMultipleValueTestMethodInfo();
            var disposalTracker = new DisposalTracker();

            // Act
            var testData = (await sut.GetData(testMethod, disposalTracker))
                .Select(row => row.GetData())
                .ToArray();

            // Assert
            Assert.Collection(testData,
                arguments =>
                {
                    Assert.Equal(3, arguments.Length);
                    Assert.Equal("value-one", arguments[0]);
                    Assert.NotEqual(0, arguments[1]);
                    Assert.NotEqual(0, arguments[2]);
                },
                arguments =>
                {
                    Assert.Equal(3, arguments.Length);
                    Assert.Equal("value-two", arguments[0]);
                    Assert.NotEqual(0, arguments[1]);
                    Assert.NotEqual(0, arguments[2]);
                },
                arguments =>
                {
                    Assert.Equal(3, arguments.Length);
                    Assert.Equal("value-three", arguments[0]);
                    Assert.NotEqual(0, arguments[1]);
                    Assert.NotEqual(0, arguments[2]);
                });
        }

        [Fact]
        public async ValueTask GeneratesTestDataWithInjectedParameters()
        {
            // Arrange
            const string memberName = nameof(TestTypeWithMethodData.GetDataForTestWithFrozenParameter);
            var sut = new MemberAutoDataAttribute(memberName);
            var testMethod = TestTypeWithMethodData.GetTestWithFrozenParameter();
            var disposalTracker = new DisposalTracker();
            var expected = new[]
            {
                new object[] { "value-one", "value-two", "value-two" },
                new object[] { "value-two", "value-three", "value-three" },
                new object[] { "value-three", "value-one", "value-one" }
            };

            // Act
            var testData = (await sut.GetData(testMethod, disposalTracker))
                .Select(row => row.GetData())
                .ToArray();

            // Assert
            Assert.Equal(testData, expected);
        }

        [Fact]
        public async ValueTask AutoGeneratesValuesForFrozenParameters()
        {
            // Arrange
            const string memberName = nameof(TestTypeWithMethodData.GetSingleStringValueTestData);
            var sut = new MemberAutoDataAttribute(memberName);
            var testMethod = TestTypeWithMethodData.GetTestWithFrozenParameter();
            var disposalTracker = new DisposalTracker();

            // Act
            var testData = (await sut.GetData(testMethod, disposalTracker))
                .Select(row => row.GetData());

            // Assert
            Assert.Collection(testData,
                arguments =>
                {
                    Assert.Equal(3, arguments.Length);
                    Assert.Equal("value-one", arguments[0]);
                    Assert.NotEmpty(arguments[1].ToString());
                    Assert.Equal(arguments[1], arguments[2]);
                },
                arguments =>
                {
                    Assert.Equal(3, arguments.Length);
                    Assert.Equal("value-two", arguments[0]);
                    Assert.NotEmpty(arguments[1].ToString());
                    Assert.Equal(arguments[1], arguments[2]);
                },
                arguments =>
                {
                    Assert.Equal(3, arguments.Length);
                    Assert.Equal("value-three", arguments[0]);
                    Assert.NotEmpty(arguments[1].ToString());
                    Assert.Equal(arguments[1], arguments[2]);
                });
        }

        [Fact]
        public async ValueTask SupportsInheritedTestDataMembers()
        {
            // Arrange
            const string memberName = nameof(TestTypeWithMethodData.GetMultipleValueTestData);
            var sut = new MemberAutoDataAttribute(memberName);
            var testMethod = ChildTestTypeMethodData.GetMultipleValueTestMethodInfo();
            var disposalTracker = new DisposalTracker();
            var expected = new[]
            {
                new object[] { "value-one", 12, 23.3m },
                new object[] { "value-two", 38, 12.7m },
                new object[] { "value-three", 94, 52.21m }
            };

            // Act
            var testData = (await sut.GetData(testMethod, disposalTracker))
                .Select(row => row.GetData())
                .ToArray();

            // Assert
            Assert.Equal(testData, expected);
        }

        public static IEnumerable<object[]> TestDataWithNullValues
        {
            get
            {
                yield return new object[] { null, null };
                yield return new object[] { string.Empty, null };
                yield return new object[] { " ", null };
            }
        }

        [Theory]
        [MemberAutoData(nameof(TestDataWithNullValues))]
        public void NullTestDataReturned(string a, string b, PropertyHolder<string> c)
        {
            Assert.True(string.IsNullOrWhiteSpace(a));
            Assert.Null(b);
            Assert.NotNull(c);
        }
    }
}
