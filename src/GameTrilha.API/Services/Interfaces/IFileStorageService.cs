namespace GameTrilha.API.Services.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadImageAsync(string base64Image, string fileName);
    Task<string> UploadFileAsync(Stream file, string containerName, string fileName);
}