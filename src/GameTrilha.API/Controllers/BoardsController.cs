using GameTrilha.API.Contexts.Repositories;
using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.ViewModels.BoardViewModels;
using GameTrilha.Domain.Entities;
using GameTrilha.Domain.Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GameTrilha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoardsController : ControllerBase
{
    private readonly ILogger<BoardsController> _logger;
    private readonly IBoardRepository _boardRepository;
    private readonly IFileStorageService _fileStorageService;

    public BoardsController(ILogger<BoardsController> logger, IBoardRepository boardRepository, IFileStorageService fileStorageService)
    {
        _logger = logger;
        _boardRepository = boardRepository;
        _fileStorageService = fileStorageService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ListBoardViewModel>> GetBoard(Guid id)
    {
        try
        {
            var board = await _boardRepository.FindByIdAsync(id);

            if (board is not null)
            {
                _logger.LogInformation("Board found with Id: {Id}", id);
                var viewModel = new ListBoardViewModel(board.Id, board.Name, board.Description, board.LineColor, board.BulletColor, board.BorderLineColor, board.BackgroundImageSrc, board.Price);
                return Ok(viewModel);
            }

            _logger.LogInformation("Board not found with Id: {Id}", id);
            return NotFound("Board not found");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting board with Id: {Id}", id);
            return BadRequest("Error getting board");
        }
    }

    [HttpGet]
    public async Task<ActionResult<ListBoardViewModel>> ListBoards()
    {
        try
        {
            var boards = await _boardRepository.ListAsync();

            var vmBoards = boards.Select(b =>
                new ListBoardViewModel(b.Id, b.Name, b.Description, b.LineColor,
                    b.BulletColor, b.BorderLineColor, b.BackgroundImageSrc, b.Price));

            return Ok(vmBoards);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing boards");
            return BadRequest("Error listing boards");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> CreateBoard(CreateBoardViewModel model)
    {
        try
        {
            _logger.LogInformation("Creating board");
            
            var url = await _fileStorageService.UploadImageAsync(model.Image, model.ImageFileName, "boards");
            var board = await _boardRepository.CreateAsync(new Board(model.Name, model.Description, model.LineColor, model.BulletColor, model.LineColor, url, model.Price));

            _logger.LogInformation("Board created with Id: {Id}", board.Id);

            var viewModel = new ListBoardViewModel(board.Id, board.Name, board.Description, board.LineColor, board.BulletColor, board.BorderLineColor, board.BackgroundImageSrc, board.Price);

            return CreatedAtAction(nameof(GetBoard), new { id = board.Id }, viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating board");
            return BadRequest("Error creating board");
        }
    }

    [HttpPost("buy/{id:guid}")]
    [Authorize]
    public async Task<ActionResult> BuyBoardSkin(Guid id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _boardRepository.BuyBoardSkinAsync(id, Guid.Parse(userId));
            return NoContent();
        }
        catch (NullReferenceException ex)
        {
            _logger.LogError(ex, "Error on buy skin. Not found skin");
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on buy skin");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("use/{id:guid}")]
    [Authorize]
    public async Task<ActionResult> UseBoardSkin(Guid id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _boardRepository.UseBoardSkinAsync(id, Guid.Parse(userId));
            return NoContent();
        }
        catch (NullReferenceException ex)
        {
            _logger.LogError(ex, "Error on change selected skin. Not found user");
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error on change selected skin. User not have this skin");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on use skin");
            return BadRequest(ex.Message);
        }
    }
}