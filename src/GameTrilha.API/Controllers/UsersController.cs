using GameTrilha.API.Helpers;
using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.ViewModels.UserViewModels;
using GameTrilha.Domain.Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameTrilha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<LobbyController> _logger;
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;

    public UsersController(ILogger<LobbyController> logger, IAuthService authService, IUserRepository userRepository)
    {
        _logger = logger;
        _authService = authService;
        _userRepository = userRepository;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ListUserViewModel>> GetUser()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await _userRepository.FindById(Guid.Parse(userId));

            if (user is null)
            {
                _logger.LogWarning("Invalid get user by token Id {userId}", userId);
                return NotFound();
            }

            _logger.LogInformation("User {userId} getted by token", userId);

            var userModel = new ListUserViewModel(user.Id, user.Name, user.Email, user.Balance, user.Avatar, user.Roles.Select(x => x.Role.Name).ToList());
            return Ok(userModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the user");
            return BadRequest();
        }
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ListUserViewModel>> GetUserById(Guid id)
    {
        try
        {
            var user = await _userRepository.FindById(id);

            user.ThrowIfNull("User not found");

            var userModel = new ListUserViewModel(user.Id, user.Name, user.Email, user.Balance, user.Avatar, user.Roles.Select(x => x.Role.Name).ToList());
            return Ok(userModel);
        }
        catch (NullReferenceException)
        {
            _logger.LogWarning("User not found by Id {userId}", id);
            return NotFound("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the user");
            return BadRequest();
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginRequestViewModel login)
    {
        try
        {
            var user = await _authService.Login(login.Email, login.Password);

            var token = _authService.GenerateToken(user);

            return Ok(token);
        }
        catch (NullReferenceException)
        {
            _logger.LogWarning("Invalid credentials for user {email}", login.Email);
            return NotFound("Bad Credentials");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while authenticate the user");
            return BadRequest();
        }
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<ListUserViewModel>> CreateUser(CreateUserViewModel model)
    {
        try
        {
            _logger.LogInformation("Creating user {email}", model.Email);

            var password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var user = await _userRepository.Create(model.Name, model.Email, password, model.Avatar);

            _logger.LogInformation("User {email} created", model.Email);

            var userModel = new ListUserViewModel(user.Id, user.Name, user.Email, user.Balance, user.Avatar, user.Roles.Select(r => r.Role.Name).ToList());

            return CreatedAtAction(nameof(GetUserById), new { id = userModel.Id }, userModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the user");
            return Conflict("User already exists");
        }
    }

    [AllowAnonymous]
    [HttpGet("recover-password/{email}")]
    public async Task<ActionResult> RequestRecoverPassword(string email)
    {
        try
        {
            _logger.LogInformation("Requesting recover password for user {email}", email);
            await _authService.RequestRecoverPasswordAsync(email);
            return NoContent();
        }
        catch (NullReferenceException)
        {
            _logger.LogWarning("User not found by email {email}", email);
            return NotFound("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while requesting recover password");
            return BadRequest(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("recover-password")]
    public async Task<ActionResult> RecoverPassword(RecoverPasswordViewModel model)
    {
        try
        {
            var success = await _authService.RecoverPasswordAsync(model.Email, model.Code, model.Password);

            if (!success) return Unauthorized("Invalid code");

            _logger.LogInformation("Password recovered for user {email}", model.Email);

            return NoContent();
        }
        catch (NullReferenceException)
        {
            _logger.LogWarning("User not found by email {email}", model.Email);
            return NotFound("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while requesting recover password");
            return BadRequest(ex.Message);
        }
    }
}