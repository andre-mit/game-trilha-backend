namespace GameTrilha.Domain.Entities;

public class Board
{
    public Guid Id { get; set; }
    public string StrokeColor { get; set; }
    public string BulletColor { get; set; }
    public string BackgroundImageSrc { get; set; }
    public double Price { get; set; }

    public ICollection<User> Users { get; set; }

    public Board(Guid id, string strokeColor, string bulletColor, string backgroundImageSrc, double price)
    {
        Id = id;
        StrokeColor = strokeColor;
        BulletColor = bulletColor;
        BackgroundImageSrc = backgroundImageSrc;
        Price = price;
    }
}