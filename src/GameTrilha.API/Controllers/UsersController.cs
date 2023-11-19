using System.Security.Claims;
using GameTrilha.API.Contexts.Repositories;
using GameTrilha.API.Helpers;
using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.ViewModels.BoardViewModels;
using GameTrilha.API.ViewModels.SkinViewModels;
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

            var userModel = new ListUserViewModel(user.Id, user.Name, user.Email, user.Balance, user.Roles.Select(x => x.Role.Name).ToList());
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

            var userModel = new ListUserViewModel(user.Id, user.Name, user.Email, user.Balance, user.Roles.Select(x => x.Role.Name).ToList());
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
            var user = await _userRepository.Create(model.Name, model.Email, password);

            _logger.LogInformation("User {email} created", model.Email);

            var userModel = new ListUserViewModel(user.Id, user.Name, user.Email, user.Balance, user.Roles.Select(r => r.Role.Name).ToList());

            return CreatedAtAction(nameof(GetUserById), new { id = userModel.Id }, userModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the user");
            return Conflict("User already exists");
        }
    }

    [Authorize]
    [HttpGet("inventory")]
    public async Task<ActionResult<ListInventoryViewModel>> ListInventory()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var boards = await _userRepository.ListBoards(userId);
            var skins = await _userRepository.ListSkins(userId);

            var vmBoards = boards!.Select(b =>
            new ListBoardViewModel(b.Id, b.Name, b.Description, b.LineColor,
                b.BulletColor, b.BorderLineColor, b.BackgroundImageSrc, b.Price));

            var vmSkins = skins!.Select(s =>
            new ListSkinViewModel(s.Id, s.Name, s.Src, s.Description,
                s.Price));

            var viewModel = new ListInventoryViewModel(vmBoards.ToList(), vmSkins.ToList());

            return Ok(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while listing skins");
            return BadRequest();
        }
    }

    [Authorize]
    [HttpGet("skinRemaining")]
    public async Task<ActionResult<ListInventoryViewModel>> ListSkinRemaining()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var boards = await _userRepository.ListBoardsRemaining(userId);
            var skins = await _userRepository.ListSkinsRemaining(userId);

            var vmBoards = boards!.Select(b =>
                new ListBoardViewModel(b.Id, b.Name, b.Description, b.LineColor,
                    b.BulletColor, b.BorderLineColor, b.BackgroundImageSrc, b.Price));

            var vmSkins = skins!.Select(s =>
                new ListSkinViewModel(s.Id, s.Name, s.Src, s.Description,
                 s.Price));

            var viewModel = new ListInventoryViewModel(vmBoards.ToList(), vmSkins.ToList());
            return Ok(viewModel);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while listing skins");
            return BadRequest();
        }

    }

}

