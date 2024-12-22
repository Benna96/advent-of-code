using Flurl.Http.Testing;

namespace AdventOfCode.Tests;

public sealed class HttpFixture : IDisposable
{
    public readonly HttpTest HttpTest = new();

    public void Dispose()
    {
        HttpTest.Dispose();
    }
}
