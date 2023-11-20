using GameTrilha.Domain.ValueObjects;

namespace GameTrilha.API.ViewModels.UserViewModels;

public class ListUserViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Balance { get; set; }
    public UserAvatar Avatar { get; set; }
    public List<string> Roles { get; set; }

    public ListUserViewModel(Guid id, string name, string email, int balance, UserAvatar avatar, List<string>? roles = null)
    {
        Id = id;
        Name = name;
        Email = email;
        Roles = roles ?? new List<string>();
        Avatar = avatar;
        Balance = balance;
    }
}