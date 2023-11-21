using GameTrilha.Domain.ValueObjects;

namespace GameTrilha.Domain.DTOs.Users;

public class RankingProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public UserAvatar Avatar { get; set; }
    public int Score { get; set; }
    public int Position { get; set; }

    public RankingProfile(Guid id, string name, UserAvatar avatar, int score, int position)
    {
        Id = id;
        Name = name;
        Avatar = avatar;
        Score = score;
        Position = position;
    }
}