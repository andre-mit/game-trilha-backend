namespace GameTrilha.API.Services.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadImageAsync(string base64Image, string fileName, string? folder = null);
    Task<string> UploadFileAsync(Stream file, string containerName, string fileName);
}