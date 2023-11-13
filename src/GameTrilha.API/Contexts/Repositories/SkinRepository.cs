using GameTrilha.Domain.DTOs.Skins;
using GameTrilha.Domain.Entities;
using GameTrilha.Domain.Entities.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GameTrilha.API.Contexts.Repositories;

public class SkinRepository : ISkinRepository
{
    private readonly TrilhaContext _context;

    public SkinRepository(TrilhaContext context)
    {
        _context = context;
    }

    public async Task<List<Skin>> ListAsync()
    {
        return await _context.Skins.AsNoTracking().ToListAsync();
    }

    public async Task<List<SkinDetails>> ListAsync(Guid userId)
    {
        return await _context.Skins
            .Include(x => x.Users)
            .AsNoTracking()
            .Select(x =>
                new SkinDetails(x.Id, x.Name, x.Src, x.Description, x.Price,
                    x.Users.Select(u => u.Id).Contains(userId))
            ).ToListAsync();
    }

    public async Task<Skin?> FindByIdAsync(Guid id)
    {
        return await _context.Skins.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<SkinDetails?> FindByIdAsync(Guid id, Guid userId)
    {
        return await _context.Skins
            .Include(x => x.Users)
            .AsNoTracking()
            .Select(x =>
                new SkinDetails(x.Id, x.Name, x.Src, x.Description, x.Price,
                        x.Users.Select(u => u.Id).Contains(userId)))
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Skin> CreateAsync(Skin skin)
    {
        await _context.Skins.AddAsync(skin);
        await _context.SaveChangesAsync();
        return skin;
    }

    public async Task<Skin> UpdateAsync(Skin skin)
    {
        var skinToUpdate = await _context.Skins.FirstOrDefaultAsync(x => x.Id == skin.Id) ?? throw new NullReferenceException("Skin not found");

        skinToUpdate.Name = skin.Name;
        skinToUpdate.Description = skin.Description;
        skinToUpdate.Price = skin.Price;
        skinToUpdate.Src = skin.Src;
        await _context.SaveChangesAsync();
        return skinToUpdate;
    }

    public async Task DeleteAsync(Guid id)
    {
        var skin = await _context.Skins.FirstOrDefaultAsync(x => x.Id == id) ?? throw new NullReferenceException("Skin not found");

        _context.Skins.Remove(skin);
        await _context.SaveChangesAsync();

    }
}