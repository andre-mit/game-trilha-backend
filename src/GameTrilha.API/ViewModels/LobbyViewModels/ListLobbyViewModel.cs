using GameTrilha.API.Services;

namespace GameTrilha.API.ViewModels.LobbyViewModels;

public record ListLobbyViewModel(string Name, string[] Players, GameService.Game.GameState State);