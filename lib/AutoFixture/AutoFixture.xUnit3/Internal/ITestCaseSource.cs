using System.Collections.Generic;
using System.Reflection;
using Xunit.Sdk;

namespace AutoFixture.Xunit3.Internal
{
    /// <summary>
    /// Exposes the factory method for a sequence of test cases.
    /// </summary>
    public interface ITestCaseSource
    {
        /// <summary>
        /// Returns the test cases provided by the source.
        /// </summary>
        /// <param name="method">The target method for which to provide the arguments.</param>
        /// <param name="disposalTracker">The disposal tracker used to dispose the data.</param>
        /// <returns>Returns a sequence of argument collections.</returns>
        IEnumerable<object[]> GetTestCases(MethodInfo method, DisposalTracker disposalTracker);
    }
}