using GameTrilha.GameDomain.Entities;
using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.ViewModels.GameViewModels;

public class ListGameDetailsViewModel
{
    public string GameId { get; set; }
    public Track[] Board { get; set; }
    public Color PlayerColor { get; set; }
    public Color Turn { get; set; }

    public Dictionary<Color, byte> PendingPieces { get; set; }

    public ListGameDetailsViewModel(string gameId, Track[] board, Color playerColor, Color turn, Dictionary<Color, byte> pendingPieces)
    {
        GameId = gameId;
        Board = board;
        PlayerColor = playerColor;
        Turn = turn;
        PendingPieces = pendingPieces;
    }
}