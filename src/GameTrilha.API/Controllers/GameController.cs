using System.Security.Claims;
using GameTrilha.API.Services;
using GameTrilha.API.ViewModels.GameViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameTrilha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    [Authorize]
    public ActionResult<ListGameDetailsViewModel> GetGameDetails()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var (id, game) = GameService.Games.FirstOrDefault(x => x.Value.Players.ContainsKey(userId));
            if (game == null || game.State == GameService.Game.GameState.Waiting)
                return NotFound("Player are not in a game");

            var viewModel =
                new ListGameDetailsViewModel(id, game.Board!.Tracks, game.Board.Players[userId], game.Board.Turn, game.Board.PendingPieces);

            return viewModel;
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}