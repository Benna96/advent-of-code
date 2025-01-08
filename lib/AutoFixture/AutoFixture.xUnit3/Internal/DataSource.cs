using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit.Sdk;

namespace AutoFixture.Xunit3.Internal
{
    /// <summary>
    /// The base class for test data sources.
    /// </summary>
    [SuppressMessage("Design", "CA1010:Generic interface should also be implemented",
        Justification = "The type is not a collection.")]
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix",
        Justification = "The type is not a collection.")]
    public abstract class DataSource : IDataSource
    {
        /// <summary>
        /// Gets the test data provided by the source.
        /// </summary>
        /// <param name="disposalTracker">The disposal tracker used to dispose the data.</param>
        /// <returns>Returns a sequence of argument collections.</returns>
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        protected abstract IEnumerable<object[]> GetData(DisposalTracker disposalTracker);

        /// <summary>
        /// Returns the test data provided by the source.
        /// </summary>
        /// <param name="method">The target method for which to provide the arguments.</param>
        /// <param name="disposalTracker">The disposal tracker used to dispose the data.</param>
        /// <returns>Returns a sequence of argument collections.</returns>
        public IEnumerable<object[]> GetData(MethodInfo method, DisposalTracker disposalTracker)
        {
            if (method is null) throw new ArgumentNullException(nameof(method));

            return GetTestDataEnumerable();

            IEnumerable<object[]> GetTestDataEnumerable()
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 0)
                {
                    // If the method has no parameters, a single test run is enough.
                    yield return Array.Empty<object>();
                    yield break;
                }

                var enumerable = this.GetData(disposalTracker)
                    ?? throw new InvalidOperationException("The source member yielded no test data.");

                foreach (var testData in enumerable)
                {
                    if (testData is null)
                        throw new InvalidOperationException("The source member yielded a null test data.");

                    if (testData.Length > parameters.Length)
                        throw new InvalidOperationException("The number of arguments provided exceeds the number of parameters.");

                    yield return testData;
                }
            }
        }
    }
}