using GameTrilha.Domain.Entities;

namespace GameTrilha.Domain.DTOs.Users;

public class BoardDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string LineColor { get; set; }

    public string BorderLineColor { get; set; }

    public string BulletColor { get; set; }

    public string? ImageSrc { get; set; }

    public static implicit operator BoardDetails?(Board? board)
    {
        return board is null ? null : new BoardDetails
        {
            Id = board.Id,
            Name = board.Name,
            LineColor = board.LineColor,
            BorderLineColor = board.BorderLineColor,
            BulletColor = board.BulletColor,
            ImageSrc = board.BackgroundImageSrc
        };
    }   
}