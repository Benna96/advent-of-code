﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit.Sdk;

namespace AutoFixture.Xunit3.Internal
{
    /// <summary>
    /// The base class for test case sources.
    /// </summary>
    [SuppressMessage("Design", "CA1010:Generic interface should also be implemented",
        Justification = "The type is not a collection.")]
    [SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix",
        Justification = "The type is not a collection.")]
    public abstract class TestCaseSource : ITestCaseSource
    {
        /// <summary>
        /// Gets the test cases provided by the source.
        /// </summary>
        /// <param name="disposalTracker">The disposal tracker used to dispose the data.</param>
        /// <returns>Returns a sequence of argument collections.</returns>
        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        protected abstract IEnumerable<object[]> GetTestData(DisposalTracker disposalTracker);

        /// <summary>
        /// Returns the test cases provided by the source.
        /// </summary>
        /// <param name="method">The target method for which to provide the arguments.</param>
        /// <param name="disposalTracker">The disposal tracker used to dispose the data.</param>
        /// <returns>Returns a sequence of argument collections.</returns>
        public IEnumerable<object[]> GetTestCases(MethodInfo method, DisposalTracker disposalTracker)
        {
            if (method is null) throw new ArgumentNullException(nameof(method));

            return GetTestCasesEnumerable();

            IEnumerable<object[]> GetTestCasesEnumerable()
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 0)
                {
                    // If the method has no parameters, a single test run is enough.
                    yield return Array.Empty<object>();
                    yield break;
                }

                var enumerable = this.GetTestData(disposalTracker)
                    ?? throw new InvalidOperationException("The source member yielded no test data.");

                foreach (var testCase in enumerable)
                {
                    if (testCase is null)
                        throw new InvalidOperationException("The source member yielded a null test case.");

                    if (testCase.Length > parameters.Length)
                        throw new InvalidOperationException("The number of arguments provided exceeds the number of parameters.");

                    yield return testCase;
                }
            }
        }
    }
}