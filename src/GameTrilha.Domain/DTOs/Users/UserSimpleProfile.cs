using GameTrilha.Domain.ValueObjects;

namespace GameTrilha.Domain.DTOs.Users;

public class UserSimpleProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public UserAvatar Avatar { get; set; }
    public string? Pieces { get; set; }
    public BoardDetails? Board { get; set; }

    public UserSimpleProfile(Guid id, string name, UserAvatar avatar, string? pieces, BoardDetails? board)
    {
        Id = id;
        Name = name;
        Avatar = avatar;
        Pieces = pieces;
        Board = board;
    }
}