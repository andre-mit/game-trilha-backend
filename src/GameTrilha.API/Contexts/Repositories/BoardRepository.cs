using GameTrilha.Domain.Entities;
using GameTrilha.Domain.Entities.Repositories;
using Microsoft.EntityFrameworkCore;

namespace GameTrilha.API.Contexts.Repositories;

public class BoardRepository : IBoardRepository
{
    private readonly TrilhaContext _context;

    public BoardRepository(TrilhaContext context) => _context = context;

    public async Task<Board?> FindByIdAsync(Guid id)
    {
        return await _context.Boards.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Board>> ListAsync()
    {
        return await _context.Boards.AsNoTracking().ToListAsync();
    }

    public async Task<Board> CreateAsync(Board board)
    {
        await _context.Boards.AddAsync(board);
        await _context.SaveChangesAsync();
        return board;
    }
}