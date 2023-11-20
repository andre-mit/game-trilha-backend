using GameTrilha.API.Helpers;
using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.SetupConfigurations.Models;
using GameTrilha.API.ViewModels.UserViewModels;
using GameTrilha.Domain.Entities;
using GameTrilha.Domain.Entities.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GameTrilha.API.Services;

public class AuthService : IAuthService
{
    private readonly JwtOptions _jwtConfig;
    private readonly ILogger<AuthService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IEmailSenderService _emailSenderService;


    public AuthService(IOptions<JwtOptions> jwtConfig, ILogger<AuthService> logger, IUserRepository userRepository, IEmailSenderService emailSenderService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _emailSenderService = emailSenderService;
        _jwtConfig = jwtConfig.Value;
    }

    /// <summary>
    /// Generates a JWT token for the given user.
    /// </summary>
    /// <summary xml:lang="pt-BR">
    /// Gera um token JWT para o usuário informado.
    /// </summary>
    /// <param name="user">The user to generate the token for.</param>
    /// <returns>The generated token.</returns>
    /// <seealso cref="Login(string, string)"/>
    public string GenerateToken(ListUserViewModel user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.Key);

        var roles = user.Roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Email, user.Email)
        };
        claims.AddRange(roles);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(_jwtConfig.Expires ?? 1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Authenticates a user with the given email and password.
    /// </summary>
    /// <summary xml:lang="pt-BR">
    /// Autentica um usuário com o email e senha informados.
    /// </summary>
    /// <param name="email">The email of the user to authenticate.</param>
    /// <param name="password">The password of the user to authenticate.</param>
    /// <returns>The authenticated user.</returns>
    /// <exception cref="NullReferenceException">Thrown when the user is not found or the password is incorrect.</exception>
    /// <seealso cref="GenerateToken(ListUserViewModel)"/>
    public async Task<ListUserViewModel> Login(string email, string password)
    {
        var user = await _userRepository.FindByEmail(email);

        if (user.IsNull() || !BCrypt.Net.BCrypt.Verify(password, user!.Password))
        {
            _logger.LogError("User not found {Email}", email);
            throw new NullReferenceException("User not found");
        }

        var roles = user.Roles.Select(x => new Role { Id = x.RoleId, Name = x.Role.Name });

        _logger.LogInformation("User {Email} logged in", email);
        return new ListUserViewModel(user.Id, user.Name, user.Email, user.Balance, user.Avatar, roles.Select(x => x.Name).ToList());
    }

    public async Task<bool> RecoverPasswordAsync(string email, string code, string newPassword)
    {
        var user = await _userRepository.FindByEmail(email);

        user.ThrowIfNull("User not found");

        return await _userRepository.UseRecoveryPasswordCodeAsync(email, code, BCrypt.Net.BCrypt.HashPassword(newPassword));
    }

    public async Task RequestRecoverPasswordAsync(string email)
    {
        var user = await _userRepository.FindByEmail(email);

        user.ThrowIfNull("User not found");
        
        Random generator = new();
        var code = generator.Next(0, 1000000).ToString("D6");

        _ = await _userRepository.CreateRecoveryPasswordAsync(user!.Id, code, DateTime.UtcNow.AddMinutes(10));

        await _emailSenderService.SendRecoverPasswordAsync(user.Email, user.Name, code);
    }
}