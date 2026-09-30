# Mole.StorageProviders.AzureBlob.TemporaryFile

Azure Blob Storage provider for temporary file uploads in Umbraco CMS. This package enables Umbraco deployments to store temporary uploaded files in Azure Blob Storage instead of local disk, eliminating filesystem dependencies for uploaded content.

## Why Use This Package?

When running Umbraco in containers (Docker, Kubernetes, Azure Container Instances, etc.), storing files on the local filesystem creates challenges, especially when load balancing:

- Files are lost when containers restart or scale
- Shared storage across multiple container instances is complex
- Ephemeral container filesystems aren't designed for file persistence

This package solves these problems by redirecting temporary file uploads to Azure Blob Storage, making your Umbraco deployment truly stateless and container-friendly.

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package Mole.StorageProviders.AzureBlob.TemporaryFile
```

## Configuration

Add your Azure Blob Storage connection string to `appsettings.json`:

```json
{
  "Umbraco": {
    "Storage": {
      "AzureBlob": {
        "TemporaryFile": {
          "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=your-account;AccountKey=your-key;EndpointSuffix=core.windows.net",
          "ContainerName": "umbraco-temp-uploads"
        }
      }
    }
  }
}
```

The `ContainerName` is optional and defaults to `tempfiles`

### Settings

| Setting | Required | Default | Description |
| --- | --- | --- | --- |
| `ConnectionString` | yes | | Azure Storage connection string. Either account credentials or a `BlobEndpoint=...;SharedAccessSignature=...` pair. |
| `ContainerName` | no | `tempfiles` | The blob container temporary files are stored in. |
| `ContainerRootPath` | no | *(none)* | A virtual folder inside the container to store temporary files under. Blob listing is scoped to it, so cleanup only ever sees its own files. |
| `CreateContainerIfNotExists` | no | `true` | Whether to create the container at startup when it is missing. |

### Using a container scoped shared access signature

Creating a container is an account level operation, so a shared access signature scoped to a single
container cannot do it. Some hosts only ever hand out container scoped credentials, Umbraco Cloud
among them, where the signature covers the media container and nothing else.

To run against credentials like those, point the package at the container the signature already
covers, give it a root path of its own inside that container, and turn container creation off:

```json
{
  "Umbraco": {
    "Storage": {
      "AzureBlob": {
        "TemporaryFile": {
          "ConnectionString": "BlobEndpoint=https://your-account.blob.core.windows.net/;SharedAccessSignature=sv=...",
          "ContainerName": "the-container-the-signature-covers",
          "ContainerRootPath": "temporary-files",
          "CreateContainerIfNotExists": false
        }
      }
    }
  }
}
```

The signature needs read, write, delete and list permissions. List is what the cleanup job uses to
find expired files; without it, temporary files are never removed.

Sharing a container this way means temporary files sit alongside whatever else is in it, under the
root path. `ContainerRootPath` keeps the two apart and keeps the cleanup job from enumerating
anything that is not its own, but it is a shared container, so anything else that walks the
container root will see the folder.

**Security Note:** For production environments, use Azure Key Vault, Managed Identity, or environment variables instead of storing connection strings in configuration files.

## Versioning
This package starts at version 17 to align with Umbraco's versioning scheme and other storage provider packages. This makes it easier to identify which version to install based on your Umbraco version. For example, version 17.x is compatible with Umbraco 17.


## How It Works

This package implements Umbraco's `ITemporaryFileRepository` interface, redirecting all temporary file operations to Azure Blob Storage. When files are uploaded through the Umbraco backoffice, they're stored in your configured Azure Blob container instead of the local `~/umbraco/Data/TEMP` folder.

## Requirements

- Umbraco 17.0 or higher
- Azure Storage Account

## Contributing

Contributions are welcome! Please feel free to submit issues, fork the repository, and create pull requests.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For issues, questions, or feature requests, please open an issue on the [GitHub repository](https://github.com/nikolajlauridsen/Mole.StorageProviders.AzureBlob.TemporaryFile/issues).