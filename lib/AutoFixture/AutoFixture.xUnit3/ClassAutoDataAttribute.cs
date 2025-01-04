﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.Xunit3.Internal;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace AutoFixture.Xunit3
{
    /// <summary>
    /// Provides a data source for a data theory, with the data coming from a class
    /// which must implement IEnumerable&lt;object?[]&gt;,
    /// combined with auto-generated data specimens generated by AutoFixture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
        Justification = "This attribute is the root of a potential attribute hierarchy.")]
    public class ClassAutoDataAttribute : DataAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassAutoDataAttribute"/> class.
        /// </summary>
        /// <param name="sourceType">The type of the class that provides the data.</param>
        /// <param name="parameters">The parameters passed to the data provider class constructor.</param>
        public ClassAutoDataAttribute(Type sourceType, params object[] parameters)
            : this(() => new Fixture(), sourceType, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassAutoDataAttribute"/> class.
        /// </summary>
        /// <param name="fixtureFactory">The fixture factory that provides missing data from <paramref name="sourceType"/>.</param>
        /// <param name="sourceType">The type of the class that provides the data.</param>
        /// <param name="parameters">The parameters passed to the data provider class constructor.</param>
        /// <para>
        /// This constructor overload exists to enable a derived attribute to
        /// supply a custom fixture factory that again may contain custom behavior.
        /// </para>
        /// <example>
        /// In the following example MyTestData is a class that provides test cases,
        /// that would be complicated or probably impossible to provide using other options.
        /// The missing arguments for the test are being supplied from the Fixture instance.
        /// <code>
        /// [Theory]
        /// [CustomAutoClassData(typeof(MyTestData))]
        /// public void ClassDataSuppliesExtraValues(int sum, int[] numbers, Person client)
        /// {
        ///     var actual = numbers.Sum(x => x);
        ///
        ///     Assert.Equal(sum, actual);
        ///     Assert.NotNull(client);
        /// }
        ///
        /// private class CustomAutoClassData : ClassAutoDataAttribute
        /// {
        ///     public CustomAutoClassData(Type sourceType) :
        ///         base(() => new Fixture(), sourceType)
        ///     {
        ///     }
        /// }
        ///
        /// private class MyTestData : IEnumerable&lt;object[]&gt;
        /// {
        ///     public IEnumerator&lt;object[]&gt; GetEnumerator()
        ///     {
        ///         yield return new object[] { 0, new int[0] };
        ///         yield return new object[] { 4, new int[] { 1, 2, 1} };
        ///         yield return new object[] { 23, new int [] { 0, 13, 15, -5 } };
        ///     }
        ///
        ///     IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        /// }
        /// </code>
        /// </example>
        protected ClassAutoDataAttribute(Func<IFixture> fixtureFactory, Type sourceType, params object[] parameters)
        {
            this.FixtureFactory = fixtureFactory ?? throw new ArgumentNullException(nameof(fixtureFactory));
            this.SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            this.Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        /// <summary>
        /// Gets the fixture factory that provides the missing data from <see cref="SourceType"/>.
        /// </summary>
        public Func<IFixture> FixtureFactory { get; }

        /// <summary>
        /// Gets the type of the class that provides the data.
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        /// Gets the constructor parameters for <see cref="SourceType"/>.
        /// </summary>
        public object[] Parameters { get; }

        /// <inheritdoc />
        public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(
            MethodInfo testMethod,
            DisposalTracker disposalTracker)
        {
            var source = new AutoTestCaseSource(
                this.FixtureFactory,
                new ClassTestCaseSource(this.SourceType, this.Parameters));

            var result = source
                .GetTestCases(testMethod, disposalTracker)
                .Select(this.ConvertDataRow)
                .ToArray();

            return new ValueTask<IReadOnlyCollection<ITheoryDataRow>>(result);
        }

        /// <summary>
        /// Always returns 'false', indicating that discovery of tests is
        /// not supported.
        /// </summary>
        /// <returns>false.</returns>
        public override bool SupportsDiscoveryEnumeration()
        {
            // The data return by ClassAutoDataAttribute is (like AutoFixture itself) typically
            // not 'stable'. In other words, the data often changes (e.g. string guids), and
            // therefore pre-discovery of tests decorated with this attribute is not possible.
            return false;
        }
    }
}
