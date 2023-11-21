using GameTrilha.Domain.DTOs.Users;
using GameTrilha.Domain.Entities;
using GameTrilha.Domain.Entities.Repositories;
using GameTrilha.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GameTrilha.API.Contexts.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TrilhaContext _context;

    public UserRepository(TrilhaContext context)
    {
        _context = context;
    }

    public async Task<User> Create(string name, string email, string password, UserAvatar avatar)
    {
        if (await _context.Users.AnyAsync(x => x.Email == email))
        {
            throw new Exception("Email already exists");
        }

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == "User");

        var user = new User(name, email, password, avatar);
        user.Roles = new List<UserRole> { new() { Role = role, User = user } };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public User Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
        return user;
    }

    public async Task<bool> Delete(Guid id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

        if (user == null) return false;

        _context.Users.Remove(user);
        var result = await _context.SaveChangesAsync();
        return result > 0;

    }

    public async Task<User?> FindById(Guid id)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(x => x.Roles)
                .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<UserSimpleProfile?> GetSimpleProfileByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(x => x.Skin)
            .Include(x => x.Board)
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new UserSimpleProfile(id, x.Name, x.Avatar, x.Skin.Src, x.Board))
            .FirstOrDefaultAsync();
    }

    public async Task<User?> FindByEmail(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<List<Skin>?> ListSkins(Guid id)
    {
        return await _context.Skins
           .AsNoTracking()
           .Where(x => x.Users.Select(u => u.Id).Contains(id))
           .ToListAsync();
    }

    public async Task<List<Board>?> ListBoards(Guid id)
    {
        return await _context.Boards
           .AsNoTracking()
           .Where(x => x.Users.Select(u => u.Id).Contains(id))
           .ToListAsync();
    }

    public async Task<List<Skin>?> ListSkinsRemaining(Guid id)
    {
        return await _context.Skins
           .AsNoTracking()
           .Where(x => !x.Users.Select(u => u.Id).Contains(id))
           .ToListAsync();
    }

    public async Task<List<Board>?> ListBoardsRemaining(Guid id)
    {
        return await _context.Boards
            .AsNoTracking()
            .Where(x => !x.Users.Select(u => u.Id).Contains(id))
            .ToListAsync();
    }

    public async Task<RecoveryPasswordCode> CreateRecoveryPasswordAsync(Guid userId, string code, DateTime expiresAt)
    {
        var recoveryRequests = await _context.RecoveryPasswordCodes.Where(r => r.UserId == userId).ToListAsync();

        recoveryRequests.ForEach(r => r.Locked = true);

        var recoveryPasswordCode = new RecoveryPasswordCode(code, expiresAt, false, userId);

        await _context.RecoveryPasswordCodes.AddAsync(recoveryPasswordCode);
        await _context.SaveChangesAsync();
        return recoveryPasswordCode;
    }

    public async Task<bool> UseRecoveryPasswordCodeAsync(string email, string code, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user == null) return false;

        var recoveryPasswordCode = await _context.RecoveryPasswordCodes.FirstOrDefaultAsync(x => x.UserId == user.Id && x.Code == code);

        if (recoveryPasswordCode == null || recoveryPasswordCode.Locked || recoveryPasswordCode.ExpiresAt < DateTime.UtcNow) return false;

        user.Password = newPassword;
        recoveryPasswordCode.Locked = true;

        _context.Users.Update(user);
        _context.RecoveryPasswordCodes.Update(recoveryPasswordCode);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<UserSimpleProfile>> GetSimpleProfileByIdsAsync(Guid[] ids)
    {
        return await _context.Users
            .Include(x => x.Skin)
            .Include(x => x.Board)
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(x => new UserSimpleProfile(x.Id, x.Name, x.Avatar, x.Skin.Src, x.Board))
            .ToListAsync();
    }

    public async Task<(Guid? selectedSkin, Guid? selectedBoard)> GetSelectedSkinAndBoard(Guid userId)
    {
        var user = await _context.Users
            .Include(x => x.Skin)
            .Include(x => x.Board)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId) ?? throw new NullReferenceException();
        return (user.Skin?.Id, user.Board?.Id);
    }

    public async Task<bool> IncreaseScoreAsync(Guid id, int score)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) return false;

        try
        {
            user.AddScore(score);
        }
        catch (Exception)
        {
            return false;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DecreaseScoreAsync(Guid id, int score)
    {
        var user = await _context.Users.AsTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) return false;

        try
        {
            user.RemoveScore(score);
        }
        catch (Exception)
        {
            return false;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<RankingProfile>> GetRankingAsync()
    {
        var ranking = await _context.Users
            .AsNoTracking()
            .OrderByDescending(x => x.Score)
            .Where(x => x.Score > 0)
            .Select(x => new RankingProfile(x.Id, x.Name, x.Avatar, x.Score, 0))
            .ToListAsync();

        for (var i = 0; i < ranking.Count; i++)
        {
            ranking[i].Position = i + 1;
        }

        return ranking;
    }

    public async Task<RankingProfile> GetRankingByUserAsync(Guid userId)
    {
        var allProfiles = await _context.Users
            .AsNoTracking()
            .OrderByDescending(x => x.Score)
            .Select(x => new RankingProfile(x.Id, x.Name, x.Avatar, x.Score, 0))
            .ToListAsync();

        for (var i = 0; i < allProfiles.Count; i++)
        {
            allProfiles[i].Position = i + 1;
        }

        var profile = allProfiles.FirstOrDefault(x => x.Id == userId);

        return profile ?? throw new NullReferenceException();
    }
}