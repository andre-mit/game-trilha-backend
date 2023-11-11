namespace GameTrilha.Domain.Entities.Repositories;

public interface ISkinRepository
{
    Task<List<Skin>> ListAsync();
    Task<Skin?> FindByIdAsync(Guid id);
    Task<Skin> CreateAsync(Skin skin);
    Task<Skin> UpdateAsync(Skin skin);
    Task DeleteAsync(Guid id);
}