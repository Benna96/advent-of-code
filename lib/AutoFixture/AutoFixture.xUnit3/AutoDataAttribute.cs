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
    /// Provides auto-generated data specimens generated by AutoFixture as an extension to
    /// xUnit.net's Theory attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
        Justification = "This attribute is the root of a potential attribute hierarchy.")]
    public class AutoDataAttribute : DataAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoDataAttribute" /> class.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This constructor overload initializes the <see cref="Fixture" /> to an instance of
        /// <see cref="Fixture" />.
        /// </para>
        /// </remarks>
        public AutoDataAttribute()
            : this(() => new Fixture())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoDataAttribute" /> class
        /// with the supplied <paramref name="fixtureFactory" />. Fixture will be created
        /// on demand using the provided factory.
        /// </summary>
        /// <param name="fixtureFactory">The fixture factory used to construct the fixture.</param>
        protected AutoDataAttribute(Func<IFixture> fixtureFactory)
        {
            this.FixtureFactory = fixtureFactory ?? throw new ArgumentNullException(nameof(fixtureFactory));
        }

        /// <summary>
        /// Gets the fixture factory.
        /// </summary>
        public Func<IFixture> FixtureFactory { get; }

        /// <summary>
        /// Returns the data to be used to test the theory.
        /// </summary>
        /// <param name="testMethod">The method that is being tested.</param>
        /// <param name="disposalTracker">The disposal tracker used to dispose the data.</param>
        /// <returns>The theory data generated by <see cref="Fixture" />.</returns>
        public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(
            MethodInfo testMethod,
            DisposalTracker disposalTracker)
        {
            if (testMethod is null) throw new ArgumentNullException(nameof(testMethod));

            var result = new AutoTestCaseSource(this.FixtureFactory)
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
            // The data return by AutoDataAttribute is (like AutoFixture itself) typically
            // not 'stable'. In other words, the data often changes (e.g. string guids), and
            // therefore pre-discovery of tests decorated with this attribute is not possible.
            return false;
        }
    }
}
