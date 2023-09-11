using GameTrilha.GameDomain.Enums;

namespace GameTrilha.GameDomain.Entities;

public class Track
{
    public Place[,] Places { get; set; } = new Place[3, 3];

    public bool PlaceAvailable(byte line, byte column)
    {
        return Places[line, column].Piece is null;
    }

    public bool MatchPiece(Color color, byte line, byte column)
    {
        var piece = Places[line, column].Piece;
        return piece is not null && piece.Color == color;
    }

    public bool Moinho(Piece piece, byte line, byte column)
    {
        var place = Places[line, column];
        if (place.Piece is null || place.Piece?.Id != piece.Id)
            return false;

        var cont = 0;
        for (var i = 0; i < 3; i++)
        {
            place = Places[line, i];
            if (place?.Piece is null)
                break;
            if (place.Piece?.Color == piece.Color) cont++;
        }

        if (cont == 3) return true;

        cont = 0;
        for (var i = 0; i < 3; i++)
        {
            place = Places[i, column];
            if (place?.Piece is null)
                break;
            if (place.Piece?.Color == piece.Color) cont++;
        }

        return cont == 3;
    }
}