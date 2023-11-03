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
    public string StrokeColor { get; set; }
    [MaxLength(7)]
    public string BulletColor { get; set; }
    [MaxLength]
    public string BackgroundImageSrc { get; set; }
    public double Price { get; set; }

    public ICollection<User> Users { get; set; }

    public Board()
    {
        
    }

    public Board(string name, string? description, string strokeColor, string bulletColor, string backgroundImageSrc, double price)
    {
        Name = name;
        Description = description;
        StrokeColor = strokeColor;
        BulletColor = bulletColor;
        BackgroundImageSrc = backgroundImageSrc;
        Price = price;
    }

    public Board(Guid id, string name, string? description, string strokeColor, string bulletColor, string backgroundImageSrc, double price)
    {
        Id = id;
        Name = name;
        Description = description;
        StrokeColor = strokeColor;
        BulletColor = bulletColor;
        BackgroundImageSrc = backgroundImageSrc;
        Price = price;
    }
}