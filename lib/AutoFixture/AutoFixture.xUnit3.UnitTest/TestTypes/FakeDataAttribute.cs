using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace AutoFixture.Xunit3.UnitTest.TestTypes
{
    public class FakeDataAttribute : DataAttribute
    {
        private readonly MethodInfo expectedMethod;
        private readonly IEnumerable<object[]> output;

        public FakeDataAttribute(MethodInfo expectedMethod, IEnumerable<object[]> output)
        {
            this.expectedMethod = expectedMethod;
            this.output = output;
        }

        public /*override*/ IEnumerable<object[]> GetData(MethodInfo methodUnderTest)
        {
            Assert.Equal(this.expectedMethod, methodUnderTest);

            return this.output;
        }

        public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
        {
            return new ValueTask<IReadOnlyCollection<ITheoryDataRow>>(this.GetData(testMethod)
                .Select(data => new TheoryDataRow(data))
                .ToArray());
        }

        public override bool SupportsDiscoveryEnumeration()
        {
            return false;
        }
    }
}
