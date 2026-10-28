using Azure.Storage.Blobs;

namespace Mole.StorageProviders.AzureBlob.TemporaryFile.Factories;

public interface ITemporaryBlobClientFactory
{
    BlobServiceClient GetBlobServiceClient();

    /// <summary>
    /// Gets a client for the named container.
    /// </summary>
    /// <remarks>
    /// Asking the service client for a container appends the name to whatever path the connection
    /// string's endpoint already carries. Where that endpoint is itself a container URL, as a host
    /// handing out container scoped credentials will give you, the name ends up in the path twice.
    /// The result still reads and writes, because a blob name may contain slashes, but listing it
    /// fails: the path is a blob rather than a container.
    /// </remarks>
    BlobContainerClient GetBlobContainerClient(string containerName)
        => GetBlobServiceClient().GetBlobContainerClient(containerName);
}
