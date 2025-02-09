#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Sdk;

namespace AutoFixture.Xunit3.Internal
{
    /// <summary>
    /// Combines the values from a source with auto-generated values.
    /// </summary>
    public class AutoDataSource : IDataSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoDataSource"/> class.
        /// </summary>
        /// <param name="createFixture">The factory method for creating a fixture.</param>
        /// <param name="source">The source of test data to combine with auto-generated values.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="createFixture"/> is <see langword="null"/>.
        /// </exception>
        public AutoDataSource(Func<IFixture> createFixture, IDataSource? source = default)
        {
            this.CreateFixture = createFixture ?? throw new ArgumentNullException(nameof(createFixture));
            this.Source = source;
        }

        /// <summary>
        /// Gets the factory method for creating a fixture.
        /// </summary>
        public Func<IFixture> CreateFixture { get; }

        /// <summary>
        /// Gets the source of test data to combine with auto-generated values.
        /// </summary>
        public IDataSource? Source { get; }

        /// <summary>
        /// Returns the combined test data provided by the source and auto-generated values.
        /// </summary>
        /// <param name="method">The target method for which to provide the arguments.</param>
        /// <param name="disposalTracker">The disposal tracker used to dispose the data.</param>
        /// <returns>Returns a sequence of argument collections.</returns>
        public IEnumerable<object[]> GetData(MethodInfo method, DisposalTracker disposalTracker)
        {
            return this.Source is null
                ? this.GenerateValues(method)
                : this.CombineValues(method, this.Source, disposalTracker);
        }

        private IEnumerable<object[]> GenerateValues(MethodBase methodInfo)
        {
            var parameters = Array.ConvertAll(methodInfo.GetParameters(), TestParameter.From);
            var fixture = this.CreateFixture();
            yield return Array.ConvertAll(parameters, parameter => GenerateAutoValue(parameter, fixture));
        }

        private IEnumerable<object[]> CombineValues(
            MethodInfo methodInfo,
            IDataSource source,
            DisposalTracker disposalTracker)
        {
            var parameters = Array.ConvertAll(methodInfo.GetParameters(), TestParameter.From);

            foreach (object[] testData in source.GetData(methodInfo, disposalTracker))
            {
                var customizations = parameters.Take(testData.Length)
                    .Zip(testData, (parameter, value) => new Argument(parameter, value))
                    .Select(argument => argument.GetCustomization())
                    .Where(x => x is not NullCustomization);

                var fixture = this.CreateFixture();
                foreach (var customization in customizations)
                {
                    fixture.Customize(customization);
                }

                var missingValues = parameters.Skip(testData.Length)
                    .Select(parameter => GenerateAutoValue(parameter, fixture))
                    .ToArray();

                yield return testData.Concat(missingValues).ToArray();
            }
        }

        private static object GenerateAutoValue(TestParameter parameter, IFixture fixture)
        {
            var customization = parameter.GetCustomization();
            if (customization is not NullCustomization)
            {
                fixture.Customize(customization);
            }

            return fixture.Resolve(parameter.ParameterInfo);
        }
    }
}