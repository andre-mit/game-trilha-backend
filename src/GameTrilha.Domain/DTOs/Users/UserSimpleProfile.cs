using GameTrilha.Domain.ValueObjects;

namespace GameTrilha.Domain.DTOs.Users;

public class UserSimpleProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public UserAvatar Avatar { get; set; }

    public UserSimpleProfile(Guid id, string name, UserAvatar avatar)
    {
        Id = id;
        Name = name;
        Avatar = avatar;
    }
}