using GameTrilha.API.Services;

namespace GameTrilha.API.ViewModels.LobbyViewModels;

public record ListLobbyViewModel(string Name, ListUserProfileLobby[] Players, GameService.Game.GameState State);