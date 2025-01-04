using System;
using System.Collections.Generic;
using AutoFixture.Xunit3.Internal;
using Xunit.Sdk;

namespace AutoFixture.Xunit3.UnitTest.Internal
{
    public class DelegatingTestCaseSource : TestCaseSource
    {
        public IEnumerable<object[]> TestCases { get; set; } = Array.Empty<object[]>();

        protected override IEnumerable<object[]> GetTestData(DisposalTracker disposalTracker)
        {
            return this.TestCases;
        }
    }
}