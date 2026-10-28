using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Mole.StorageProviders.AzureBlob.TemporaryFile.Settings;

namespace Mole.StorageProviders.AzureBlob.TemporaryFile.Factories;

public class TemporaryBlobClientFactory : ITemporaryBlobClientFactory
{
    private readonly TemporaryFileSettings _temporaryFileSettings;

    public TemporaryBlobClientFactory(IOptions<TemporaryFileSettings> temporaryFileSettings)
    {
        _temporaryFileSettings = temporaryFileSettings.Value;
    }
    
    public BlobServiceClient GetBlobServiceClient()
    {
        var connectionString = _temporaryFileSettings.ConnectionString;
        return new BlobServiceClient(connectionString);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Built straight from the connection string rather than from the service client, so the
    /// container name replaces the endpoint's path instead of being appended to it.
    /// </remarks>
    public BlobContainerClient GetBlobContainerClient(string containerName) =>
        new(_temporaryFileSettings.ConnectionString, containerName);
}
