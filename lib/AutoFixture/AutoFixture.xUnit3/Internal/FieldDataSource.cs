using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit.Sdk;

namespace AutoFixture.Xunit3.Internal
{
    /// <summary>
    /// Encapsulates access to a field that provides test data.
    /// </summary>
    [SuppressMessage("Design", "CA1010:Generic interface should also be implemented",
        Justification = "Type is not a collection.")]
    public class FieldDataSource : DataSource
    {
        /// <summary>
        /// Creates an instance of type <see cref="FieldDataSource" />.
        /// </summary>
        /// <param name="fieldInfo">The source field info.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="fieldInfo" /> is <see langword="null" />.
        /// </exception>
        public FieldDataSource(FieldInfo fieldInfo)
        {
            this.FieldInfo = fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo));
        }

        /// <summary>
        /// Gets the source field.
        /// </summary>
        public FieldInfo FieldInfo { get; }

        /// <summary>
        /// Gets the test data from the source field.
        /// </summary>
        /// <param name="disposalTracker">The disposal tracker used to dispose the data.</param>
        /// <returns>Returns a sequence of argument collections.</returns>
        /// <exception cref="InvalidCastException">
        /// Thrown when the field does not return an enumerable value.
        /// </exception>
        protected override IEnumerable<object[]> GetData(DisposalTracker disposalTracker)
        {
            var value = this.FieldInfo.GetValue(null);
            if (value is not IEnumerable<object[]> enumerable)
                throw new InvalidCastException("Member does not return an enumerable value.");

            return enumerable;
        }
    }
}