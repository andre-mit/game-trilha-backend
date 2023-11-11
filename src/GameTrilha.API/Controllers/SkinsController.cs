using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameTrilha.API.Contexts;
using GameTrilha.Domain.Entities;
using GameTrilha.Domain.Entities.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace GameTrilha.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkinsController : ControllerBase
    {
        private readonly ILogger<SkinsController> _logger;
        private readonly ISkinRepository _skinRepository;

        public SkinsController(ILogger<SkinsController> logger, ISkinRepository skinRepository)
        {
            _logger = logger;
            _skinRepository = skinRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skin>>> GetSkins()
        {
            return await _skinRepository.ListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Skin>> GetSkin(Guid id)
        {
            var skin = await _skinRepository.FindByIdAsync(id);

            if (skin is null)
            {
                return NotFound();
            }

            return skin;
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutSkin(Skin skin)
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
        public async Task<ActionResult<Skin>> PostSkin(Skin skin)
        {
            try
            {
                var createdSkin = await _skinRepository.CreateAsync(skin);

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
