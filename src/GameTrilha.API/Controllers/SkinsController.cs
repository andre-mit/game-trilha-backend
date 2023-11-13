using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.ViewModels.SkinViewModels;
using GameTrilha.Domain.Entities;
using GameTrilha.Domain.Entities.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GameTrilha.Domain.DTOs.Skins;

namespace GameTrilha.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkinsController : ControllerBase
    {
        private readonly ILogger<SkinsController> _logger;
        private readonly ISkinRepository _skinRepository;
        private readonly IFileStorageService _fileStorageService;

        public SkinsController(ILogger<SkinsController> logger, ISkinRepository skinRepository, IFileStorageService fileStorageService)
        {
            _logger = logger;
            _skinRepository = skinRepository;
            _fileStorageService = fileStorageService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SkinDetails>>> GetSkins()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            return await _skinRepository.ListAsync(Guid.Parse(userId));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<SkinDetails>> GetSkin(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var skin = await _skinRepository.FindByIdAsync(id, Guid.Parse(userId));

            if (skin is null)
            {
                return NotFound();
            }

            return skin;
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSkin(Skin skin)
        {
            try
            {
                await _skinRepository.UpdateAsync(skin);
                return NoContent();
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, "Error on update skin. Not found skin");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on update skin.");
                return Problem(ex.Message);
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Skin>> CreateSkin(CreateSkinViewModel model)
        {
            try
            {
                _logger.LogInformation("Creating skin with name: {Name}", model.Name);

                var url = await _fileStorageService.UploadImageAsync(model.Image, model.ImageFileName, "skins");
                var skin = new Skin(model.Name, url, model.Description, model.Price);
                var createdSkin = await _skinRepository.CreateAsync(skin);

                _logger.LogInformation("Skin {Name} created with Id: {Id}", createdSkin.Name, createdSkin.Id);

                return CreatedAtAction(nameof(GetSkin), new { id = skin.Id }, createdSkin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on create skin");
                return Problem(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSkin(Guid id)
        {
            try
            {
                await _skinRepository.DeleteAsync(id);

                return NoContent();
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, "Error on delete skin. Not found skin");
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on delete skin");
                return Problem(ex.Message);
            }
        }
    }
}
