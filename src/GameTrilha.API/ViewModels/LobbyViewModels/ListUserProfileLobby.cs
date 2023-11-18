using GameTrilha.Domain.DTOs.Users;
using GameTrilha.Domain.ValueObjects;

namespace GameTrilha.API.ViewModels.LobbyViewModels;

public class ListUserProfileLobby : UserSimpleProfile
{
    public bool Moinho { get; set; }

    public ListUserProfileLobby(Guid id, string name, UserAvatar avatar, bool moinho) : base(id, name, avatar, null, null)
    {
        Moinho = moinho;
    }
}