using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AutoFixture.Xunit3.Internal
{
    /// <summary>
    /// Provides test cases from a predefined collection of values.
    /// </summary>
    [SuppressMessage("Design", "CA1010:Generic interface should also be implemented",
        Justification = "Type is not a collection.")]
    public sealed class InlineTestCaseSource : ITestCaseSource
    {
        /// <summary>
        /// Creates an instance of type <see cref="InlineTestCaseSource" />.
        /// </summary>
        /// <param name="values">The collection of inline values.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the values collection is <see langword="null" />.
        /// </exception>
        public InlineTestCaseSource(object[] values)
        {
            this.Values = values ?? throw new ArgumentNullException(nameof(values));
        }

        /// <summary>
        /// The collection of inline values.
        /// </summary>
        public object[] Values { get; }

        /// <inheritdoc />
        public IEnumerable<object[]> GetTestCases(MethodInfo method)
        {
            if (method is null) throw new ArgumentNullException(nameof(method));

            var parameters = method.GetParameters();
            if (this.Values.Length > parameters.Length)
            {
                throw new InvalidOperationException(
                    "The number of arguments provided exceeds the number of parameters.");
            }

            return new[] { this.Values };
        }
    }
}