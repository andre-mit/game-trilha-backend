using System.Security.Claims;
using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.ViewModels.UserViewModels;
using GameTrilha.Domain.Entities;
using GameTrilha.Domain.Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("{id}")]
    public async Task<ActionResult<ListUserViewModel>> GetUserById(Guid id)
    {
        try
        {
            var user = await _userRepository.FindById(id);
            var userModel = new ListUserViewModel(user.Id, user.Name, user.Email, new List<string>());
            return Ok(userModel);
        }
        catch (NullReferenceException ex)
        {
            _logger.LogError(ex, "Invalid user");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the user");
            return BadRequest();
        }
    }

    [HttpPost("auth")]
    public async Task<ActionResult> Login(LoginRequestViewModel login)
    {
        try
        {
            var user = await _authService.Login(login.Email, login.Password);

            var token = _authService.GenerateToken(user);

            return Ok(new { user, token });
        }
        catch (NullReferenceException ex)
        {
            _logger.LogError(ex, "Invalid user");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while authenticate the user");
            return BadRequest();
        }
    }

    [HttpPost]
    public async Task<ActionResult<ListUserViewModel>> CreateUser(CreateUserViewModel model)
    {
        try
        {
            var password = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var user = await _userRepository.Create(model.Name, model.Email, password);

            var userModel = new ListUserViewModel(user.Id, user.Name, user.Email, new List<string>());

            return CreatedAtAction(nameof(GetUserById), new { id = userModel.Id }, userModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the user");
            return BadRequest();
        }
    }
}