using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.ViewModels.GameViewModels;

public class PieceDetailsViewModel
{
    public Guid Id { get; set; }
    public Color Color { get; set; }
    public int Track { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }

    public PieceDetailsViewModel(Guid id, Color color, int track, int line, int column)
    {
        Id = id;
        Color = color;
        Track = track;
        Line = line;
        Column = column;
    }
}