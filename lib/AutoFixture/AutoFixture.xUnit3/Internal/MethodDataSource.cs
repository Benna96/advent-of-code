using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit.Sdk;

namespace AutoFixture.Xunit3.Internal
{
    /// <summary>
    /// Encapsulates access to a method that provides test data.
    /// </summary>
    [SuppressMessage("Design", "CA1010:Generic interface should also be implemented",
        Justification = "Type is not a collection.")]
    public class MethodDataSource : DataSource
    {
        private readonly object[] arguments;

        /// <summary>
        /// Creates an instance of type <see cref="MethodDataSource" />.
        /// </summary>
        /// <param name="methodInfo">The source method.</param>
        /// <param name="arguments">The source method arguments.</param>
        public MethodDataSource(MethodInfo methodInfo, params object[] arguments)
        {
            this.MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            this.arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
        }

        /// <summary>
        /// Gets the source method info.
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <summary>
        /// Gets the source method arguments.
        /// </summary>
        public IReadOnlyList<object> Arguments => Array.AsReadOnly(this.arguments);

        /// <inheritdoc />
        protected override IEnumerable<object[]> GetData(DisposalTracker disposalTracker)
        {
            var value = this.MethodInfo.Invoke(null, this.arguments);
            if (value is not IEnumerable<object[]> enumerable)
                throw new InvalidCastException("Member does not return an enumerable value.");

            return enumerable;
        }
    }
}