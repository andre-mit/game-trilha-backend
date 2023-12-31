﻿using System.Security.Claims;
using GameTrilha.API.Hubs;
using GameTrilha.API.Services;
using GameTrilha.API.ViewModels.GameViewModels;
using GameTrilha.Domain.Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GameTrilha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly ILogger<GamesController> _logger;
    private readonly IHubContext<GameHub> _gameHubContext;
    private readonly IUserRepository _userRepository;

    public GamesController(ILogger<GamesController> logger, IHubContext<GameHub> gameHubContext, IUserRepository userRepository)
    {
        _logger = logger;
        _gameHubContext = gameHubContext;
        _userRepository = userRepository;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ListGameDetailsViewModel>> GetGameDetails()
    {
        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var userId = Guid.Parse(userIdString);
            var (id, game) = GameService.Games.FirstOrDefault(x => x.Value.Players.ContainsKey(userId));
            if (game?.Board is null || game.State == GameService.Game.GameState.Waiting)
                return NotFound("Player are not in a game");

            var profiles = await _userRepository.GetSimpleProfileByIdsAsync(game.Players.Select(x => x.Key).ToArray());

            var pieces = new List<PieceDetailsViewModel>();
            for (var i = 0; i < 3; i++)
                for (var j = 0; j < 3; j++)
                    for (var k = 0; k < 3; k++)
                    {
                        if (j == 1 && k == 1) continue;
                        var piece = game.Board.Tracks[i].Places[j, k].Piece;
                        if (piece is null) continue;

                        pieces.Add(new PieceDetailsViewModel(piece.Id, piece.Color, i, j, k));
                    }

            var viewModel =
                new ListGameDetailsViewModel(
                    id,
                    pieces.ToArray(),
                    game.Board.Players[userId],
                    game.Board.Turn, game.Board.PendingPieces,
                    profiles.First(x => x.Id == userId),
                    profiles.First(x => x.Id != userId));

            return Ok(viewModel);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}