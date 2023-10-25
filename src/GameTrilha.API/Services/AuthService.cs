using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.SetupConfigurations.Models;
using GameTrilha.API.ViewModels.UserViewModels;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using GameTrilha.API.Contexts;
using Microsoft.EntityFrameworkCore;

namespace GameTrilha.API.Services;

public class AuthService : IAuthService
{
    private readonly JwtOptions _jwtConfig;
    private readonly ILogger<AuthService> _logger;
    private readonly TrilhaContext _context;

    public AuthService(IOptions<JwtOptions> jwtConfig, TrilhaContext context, ILogger<AuthService> logger)
    {
        _context = context;
        _logger = logger;
        _jwtConfig = jwtConfig.Value;
    }

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

    public async Task<ListUserViewModel> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            _logger.LogError("User not found {Email}", email);
            throw new NullReferenceException("User not found");
        }

        _logger.LogInformation("User {Email} logged in", email);
        return new ListUserViewModel(user.Id, user.Name, user.Email, new List<string>());
    }
}