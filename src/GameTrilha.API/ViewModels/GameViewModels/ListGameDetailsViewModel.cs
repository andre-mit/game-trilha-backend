using GameTrilha.Domain.DTOs.Users;
using GameTrilha.GameDomain.Entities;
using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.ViewModels.GameViewModels;

public class ListGameDetailsViewModel
{
    public string GameId { get; set; }
    public PieceDetailsViewModel[] Pieces { get; set; }
    public Color PlayerColor { get; set; }
    public Color Turn { get; set; }
    public UserSimpleProfile Profile { get; set; }
    public UserSimpleProfile OpponentProfile { get; set; }

    public Dictionary<Color, byte> PendingPieces { get; set; }

    public ListGameDetailsViewModel(string gameId, PieceDetailsViewModel[] pieces, Color playerColor, Color turn, Dictionary<Color, byte> pendingPieces, UserSimpleProfile profile, UserSimpleProfile opponentProfile)
    {
        GameId = gameId;
        Pieces = pieces;
        PlayerColor = playerColor;
        Turn = turn;
        PendingPieces = pendingPieces;
        Profile = profile;
        OpponentProfile = opponentProfile;
    }
}