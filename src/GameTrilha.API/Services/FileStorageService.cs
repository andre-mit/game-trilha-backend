using Azure.Storage.Blobs;
using GameTrilha.API.Services.Interfaces;
using System.Text.RegularExpressions;

namespace GameTrilha.API.Services;

public class FileStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(BlobServiceClient blobServiceClient, ILogger<FileStorageService> logger) =>
        (_blobServiceClient, _logger) = (blobServiceClient, logger);

    public async Task<string> UploadImageAsync(string base64Image, string fileName)
    {
        var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(base64Image, "");
        var bytes = Convert.FromBase64String(data);
        fileName = $"{Guid.NewGuid():N}_{fileName}";

        _logger.LogInformation("Uploading image \"{fileName}\" to Azure Blob Storage", fileName);

        var url = await UploadFileAsync(new MemoryStream(bytes), "images", fileName);

        _logger.LogInformation("Image \"{fileName}\" uploaded to Azure Blob Storage with URL: \"{URL}\"", fileName, url);

        return url;
    }

    public async Task<string> UploadFileAsync(Stream file, string containerName, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        _ = await containerClient.CreateIfNotExistsAsync();
        var blobClient = containerClient.GetBlobClient($"{Guid.NewGuid():N}_{fileName}");
        await blobClient.UploadAsync(file, true);

        return blobClient.Uri.AbsoluteUri;
    }
}