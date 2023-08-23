using GameTrilha.GameDomain.Enums;

namespace GameTrilha.GameDomain.Entities;

public class Piece
{
    public Guid Id { get; set; }
    public Color Color { get; set; }

    public Piece()
    {
        
    }

    public Piece(Color color)
    {
        Id = Guid.NewGuid();
        Color = color;
    }
}