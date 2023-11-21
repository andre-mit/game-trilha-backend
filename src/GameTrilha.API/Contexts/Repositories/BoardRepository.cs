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

    public async Task BuyBoardSkinAsync(Guid boardId, Guid userId)
    {
        var board = await _context.Boards.Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == boardId) ?? throw new NullReferenceException("Board not found");
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId) ?? throw new NullReferenceException("User not found");

        if (board.Users.Contains(user))
        {
            throw new InvalidOperationException("User already has this board");
        }

        if (user.Balance < board.Price)
        {
            throw new InvalidOperationException("User does not have enough coins");
        }


        user.RemoveBalance((int)board.Price);
        board.Users.Add(user);

        await _context.SaveChangesAsync();
    }

    public async Task UseBoardSkinAsync(Guid boardId, Guid userId)
    {
        var user = await _context.Users
            .Include(x => x.Board)
            .Include(x => x.Boards)
            .FirstOrDefaultAsync(x => x.Id == userId) ?? throw new NullReferenceException("User not found");

        if (user.Boards.All(x => x.Id != boardId))
            throw new InvalidOperationException("User does not have this board");


        if (user.Board?.Id == boardId)
            return;

        user.Board = user.Boards.First(x => x.Id == boardId);
        await _context.SaveChangesAsync();
    }
}