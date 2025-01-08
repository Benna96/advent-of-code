using System;
using System.Collections.Generic;
using AutoFixture.Xunit3.Internal;
using Xunit.Sdk;

namespace AutoFixture.Xunit3.UnitTest.Internal
{
    public class DelegatingDataSource : DataSource
    {
        public IEnumerable<object[]> TestData { get; set; } = Array.Empty<object[]>();

        protected override IEnumerable<object[]> GetData(DisposalTracker disposalTracker)
        {
            return this.TestData;
        }
    }
}