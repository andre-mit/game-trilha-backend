using GameTrilha.GameDomain.Enums;

namespace GameTrilha.GameDomain.Entities;

public class Piece
{
    public Guid Id { get; }
    public Color Color { get; }

    public Piece(Color color)
    {
        Id = Guid.NewGuid();
        Color = color;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Piece piece)
            return false;
        return Id == piece.Id && Color == piece.Color;
    }

    protected bool Equals(Piece other)
    {
        return Id.Equals(other.Id) && Color == other.Color;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, (int)Color);
    }
}