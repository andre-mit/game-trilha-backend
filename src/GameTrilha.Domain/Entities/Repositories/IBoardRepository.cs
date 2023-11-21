namespace GameTrilha.Domain.Entities.Repositories;

public interface IBoardRepository
{
    Task<Board?> FindByIdAsync(Guid id);
    Task<List<Board>> ListAsync();
    Task<Board> CreateAsync(Board board);
    Task BuyBoardSkinAsync(Guid boardId, Guid userId);
}