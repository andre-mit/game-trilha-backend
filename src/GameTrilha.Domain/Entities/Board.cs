using System.ComponentModel.DataAnnotations;

namespace GameTrilha.Domain.Entities;

public class Board
{
    public Guid Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength]
    public string? Description { get; set; }

    [MaxLength(7)]
    public string LineColor { get; set; }

    [MaxLength(7)]
    public string BorderLineColor { get; set; }

    [MaxLength(7)]
    public string BulletColor { get; set; }

    [MaxLength]
    public string BackgroundImageSrc { get; set; }
    public double Price { get; set; }

    public ICollection<User> Users { get; set; }

    public Board()
    {
        
    }

    public Board(string name, string? description, string lineColor, string bulletColor, string backgroundImageSrc, double price)
    {
        Name = name;
        Description = description;
        LineColor = lineColor;
        BulletColor = bulletColor;
        BackgroundImageSrc = backgroundImageSrc;
        Price = price;
    }

    public Board(Guid id, string name, string? description, string lineColor, string bulletColor, string backgroundImageSrc, double price)
    {
        Id = id;
        Name = name;
        Description = description;
        LineColor = lineColor;
        BulletColor = bulletColor;
        BackgroundImageSrc = backgroundImageSrc;
        Price = price;
    }
}