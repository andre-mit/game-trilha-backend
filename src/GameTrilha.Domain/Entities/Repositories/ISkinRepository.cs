using GameTrilha.Domain.DTOs.Skins;

namespace GameTrilha.Domain.Entities.Repositories;

public interface ISkinRepository
{
    Task<List<Skin>> ListAsync();
    Task<List<SkinDetails>> ListAsync(Guid userId);
    Task<Skin?> FindByIdAsync(Guid id);
    Task<SkinDetails?> FindByIdAsync(Guid id, Guid userId);
    Task<Skin> CreateAsync(Skin skin);
    Task<Skin> UpdateAsync(Skin skin);
    Task DeleteAsync(Guid id);
    Task BuySkinAsync(Guid skinId, Guid userId);
    Task UseSkinAsync(Guid skinId, Guid userId);
}