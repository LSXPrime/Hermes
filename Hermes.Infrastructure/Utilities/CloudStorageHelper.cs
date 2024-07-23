using Azure.Storage.Blobs;
using Hermes.Application.Interfaces;
using Hermes.Domain.Settings;
using Microsoft.Extensions.Options;

namespace Hermes.Infrastructure.Utilities;

public class CloudStorageHelper(IOptions<AzureStorageSettings> azureStorageSettings) : ICloudStorageHelper
{
    /// <summary>
    /// Uploads a file to Azure Blob Storage.
    /// </summary>
    /// <param name="fileStream">The stream containing the file data.</param>
    /// <param name="fileName">The original name of the file.</param>
    /// <param name="folderName">The folder name within the Azure Blob Storage container where the file should be stored.</param>
    /// <returns>
    /// The URI of the uploaded file in Azure Blob Storage.
    /// </returns>
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderName)
    {
        var blobServiceClient = new BlobServiceClient(azureStorageSettings.Value.ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(azureStorageSettings.Value.ContainerName);

        var blobName = $"{folderName}/{Guid.NewGuid()}_{fileName}";
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(fileStream, true);

        return blobClient.Uri.ToString();
    }
}