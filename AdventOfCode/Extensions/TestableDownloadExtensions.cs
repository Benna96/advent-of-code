using System.IO.Abstractions;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using FileMode = System.IO.FileMode;
using FileAccess = System.IO.FileAccess;
using FileShare = System.IO.FileShare;

// ReSharper disable once CheckNamespace
namespace Flurl.Http;

/// <summary>
/// Version of <see cref="DownloadExtensions"/> using System.IO.Abstractions.
/// </summary>
public static class TestableDownloadExtensions
{
    public static async Task<string> DownloadFileAsync(
        this IFlurlRequest request,
        IFileSystem fileSystem,
        string localPath,
        int bufferSize = 4096,
        HttpCompletionOption completionOption = HttpCompletionOption.ResponseHeadersRead,
        CancellationToken cancellationToken = default)
    {
        await using var httpStream = await request.GetStreamAsync(completionOption, cancellationToken);

        var directory = fileSystem.Path.GetDirectoryName(localPath);
        if (directory is not null && !fileSystem.Directory.Exists(directory))
            fileSystem.Directory.CreateDirectory(directory);

        await using var fileStream = fileSystem.FileStream.New(
            localPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize);

        await httpStream.CopyToAsync(fileStream, bufferSize, cancellationToken);
        return localPath;
    }
}
