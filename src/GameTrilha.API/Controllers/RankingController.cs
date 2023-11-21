using System.Security.Claims;
using GameTrilha.API.ViewModels.UserViewModels;
using GameTrilha.Domain.DTOs.Users;
using GameTrilha.Domain.Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameTrilha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RankingController : ControllerBase
{
    private readonly ILogger<LobbyController> _logger;
    private readonly IUserRepository _userRepository;

    public RankingController(ILogger<LobbyController> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<RankingProfile>> GetRanking()
    {
        try
        {
            var ranking = await _userRepository.GetRankingAsync();

            return Ok(ranking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the ranking");
            return BadRequest();
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<RankingProfile>> GetRankingByUser()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var ranking = await _userRepository.GetRankingByUserAsync(Guid.Parse(userId));
            return Ok(ranking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the ranking");
            return BadRequest();
        }
    }
}