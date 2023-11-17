namespace GameTrilha.API.Services.Interfaces;

public interface IEmailSenderService
{
    Task SendRecoverPasswordAsync(string email, string name, string code);
}