namespace Mole.StorageProviders.AzureBlob.TemporaryFile.Settings;

public class TemporaryFileSettings
{
    public string? ConnectionString { get; set; }

    public string ContainerName { get; set; } = "tempfiles";

    /// <summary>
    /// Gets or sets a virtual folder within the container that temporary files are stored under.
    /// </summary>
    /// <remarks>
    /// Not set by default, so blobs are written to the root of the container. Set this when the
    /// container is shared with something else, such as a media container, to keep temporary files
    /// separate from whatever else lives there. Blob listing is scoped to this path, so cleanup only
    /// ever sees its own files.
    /// </remarks>
    public string? ContainerRootPath { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the container is created when it does not exist.
    /// </summary>
    /// <remarks>
    /// Creating a container is an account level operation. Set this to <c>false</c> when the
    /// connection string carries a shared access signature scoped to a single container, which
    /// cannot create one, and point <see cref="ContainerName"/> at a container that already exists.
    /// </remarks>
    public bool CreateContainerIfNotExists { get; set; } = true;

    /// <summary>
    /// Gets the <see cref="ContainerRootPath"/> as a blob name prefix, or <c>null</c> when no root
    /// is configured.
    /// </summary>
    /// <remarks>
    /// Null rather than empty so it can be handed to a blob listing as "no filter", and so blob
    /// names are unchanged for anyone who has not set a root path.
    /// </remarks>
    public string? GetBlobPrefix()
    {
        var root = ContainerRootPath?.Trim('/');

        return string.IsNullOrWhiteSpace(root) ? null : root + '/';
    }
}