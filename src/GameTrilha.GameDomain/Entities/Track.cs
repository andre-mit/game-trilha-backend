using GameTrilha.GameDomain.Enums;

namespace GameTrilha.GameDomain.Entities;

public class Track
{
    public Place[,] Places { get; set; } = new Place[3, 3]
    {
        { new(), new(), new() },
        { new(), null!, new() },
        { new(), new(), new() }
    };

    public bool PlaceAvailable(byte line, byte column)
    {
        return Places[line, column].Piece is null;
    }

    public bool MatchPiece(Color color, byte line, byte column)
    {
        var piece = Places[line, column].Piece;
        return piece is not null && piece.Color == color;
    }

    public (bool, Guid[]) Moinho(Color color, byte line, byte column)
    {
        var place = Places[line, column];
        if (place.Piece is null || place.Piece?.Color != color)
            return (false, null!);

        var matches = new Guid?[] { null, null, null };
        for (var i = 0; i < 3; i++)
        {
            place = Places[line, i];
            if (place?.Piece is null)
                break;
            if (place.Piece?.Color == color) matches[i] = place.Piece.Id;
        }

        var moinho = matches.All(x => x != null);

        if (moinho) return (moinho, matches.Select(x => x!.Value).ToArray());

        matches = new Guid?[] { null, null, null };
        for (var i = 0; i < 3; i++)
        {
            place = Places[i, column];
            if (place?.Piece is null)
                break;
            if (place.Piece?.Color == color) matches[i] = place.Piece.Id;
        }

        moinho = matches.All(x => x != null);
        return moinho ? (moinho, matches.Select(x => x!.Value).ToArray()) : (false, null!);
    }
}