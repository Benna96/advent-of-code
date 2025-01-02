﻿#nullable enable
using System;
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
    /// Provides a data source for a data theory, with the data coming from one of the following sources
    /// and combined with auto-generated data specimens generated by AutoFixture:
    /// 1. A static property
    /// 2. A static field
    /// 3. A static method (with parameters)
    /// The member must return something compatible with IEnumerable&lt;object[]&gt; with the test data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [CLSCompliant(false)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
        Justification = "This attribute is the root of a potential attribute hierarchy.")]
    public class MemberAutoDataAttribute : DataAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberAutoDataAttribute" /> class.
        /// </summary>
        /// <param name="memberName">The name of the public static member on the test class that will provide the test data.</param>
        /// <param name="parameters">The parameters for the member (only supported for methods; ignored for everything else).</param>
        public MemberAutoDataAttribute(string memberName, params object[] parameters)
            : this(() => new Fixture(), default, memberName, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberAutoDataAttribute" /> class.
        /// </summary>
        /// <param name="memberType">The type declaring the source member.</param>
        /// <param name="memberName">The name of the public static member on the test class that will provide the test data.</param>
        /// <param name="parameters">The parameters for the member (only supported for methods; ignored for everything else).</param>
        public MemberAutoDataAttribute(Type? memberType, string memberName, params object[] parameters)
            : this(() => new Fixture(), memberType, memberName, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberAutoDataAttribute" /> class.
        /// </summary>
        /// <param name="fixtureFactory">The fixture factory delegate.</param>
        /// <param name="memberName">The name of the public static member on the test class that will provide the test data.</param>
        /// <param name="parameters">The parameters for the member (only supported for methods; ignored for everything else).</param>
        protected MemberAutoDataAttribute(Func<IFixture> fixtureFactory, string memberName, params object[] parameters)
            : this(fixtureFactory, default, memberName, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberAutoDataAttribute" /> class.
        /// </summary>
        /// <param name="fixtureFactory">The fixture factory delegate.</param>
        /// <param name="memberType">The type declaring the source member.</param>
        /// <param name="memberName">The name of the public static member on the test class that will provide the test data.</param>
        /// <param name="parameters">The parameters for the member (only supported for methods; ignored for everything else).</param>
        /// <exception cref="ArgumentNullException">Thrown when arguments are null.</exception>
        protected MemberAutoDataAttribute(Func<IFixture> fixtureFactory, Type? memberType, string memberName, params object[] parameters)
        {
            this.FixtureFactory = fixtureFactory ?? throw new ArgumentNullException(nameof(fixtureFactory));
            this.MemberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            this.Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            this.MemberType = memberType;
        }

        /// <summary>
        /// Gets the fixture factory that provides the missing data from <see cref="MemberType" />.
        /// </summary>
        public Func<IFixture> FixtureFactory { get; }

        /// <summary>
        /// Gets the type of the class that provides the data.
        /// </summary>
        public Type? MemberType { get; }

        /// <summary>
        /// Gets the member name.
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        /// Gets the parameters passed to the member. Only supported for static methods.
        /// </summary>
        public object[] Parameters { get; }

        /// <inheritdoc />
        public /*override*/ IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (testMethod is null) throw new ArgumentNullException(nameof(testMethod));

            var sourceType = this.MemberType ?? testMethod.DeclaringType
                ?? throw new InvalidOperationException("Source type cannot be null.");

            return new AutoTestCaseSource(
                    this.FixtureFactory,
                    new MemberTestCaseSource(sourceType, this.MemberName, this.Parameters))
                .GetTestCases(testMethod).Select(x => x.ToArray());
        }

        public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
        {
            return new ValueTask<IReadOnlyCollection<ITheoryDataRow>>(this.GetData(testMethod)
                .Select(data => new TheoryDataRow(data))
                .ToArray());
        }

        /// <summary>
        /// Always returns 'false', indicating that discovery of tests is
        /// not supported.
        /// </summary>
        /// <returns>false.</returns>
        public override bool SupportsDiscoveryEnumeration()
        {
            // The data return by MemberAutoDataAttribute is (like AutoFixture itself) typically
            // not 'stable'. In other words, the data often changes (e.g. string guids), and
            // therefore pre-discovery of tests decorated with this attribute is not possible.
            return false;
        }
    }
}
