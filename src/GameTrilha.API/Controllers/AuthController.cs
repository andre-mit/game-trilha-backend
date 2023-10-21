using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GameTrilha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<LobbyController> _logger;
    private readonly IAuthService _authService;

    public AuthController(ILogger<LobbyController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    // TODO: implements user authentication
    [HttpPost]
    public IActionResult Login(LoginRequestViewModel login)
    {
        try
        {
            var user = new ListUserViewModel(Guid.NewGuid(), login.Email.Split('@')[0], login.Email,
                new List<string> { "player" });

            var token = _authService.GenerateToken(user);

            return Ok(new { user, token });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while authenticate the user");
            return BadRequest();
        }
    }
}