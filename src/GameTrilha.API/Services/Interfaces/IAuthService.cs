using GameTrilha.API.ViewModels.UserViewModels;

namespace GameTrilha.API.Services.Interfaces;

/// <summary>
/// Service to generate JWT token
/// </summary>
public interface IAuthService
{
    string GenerateToken(ListUserViewModel user);
    Task<ListUserViewModel> Login(string email, string password);
    Task RequestRecoverPasswordAsync(string email);
    Task<bool> RecoverPasswordAsync(string email, string code, string newPassword);
}