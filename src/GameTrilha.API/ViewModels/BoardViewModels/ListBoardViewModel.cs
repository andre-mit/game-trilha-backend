namespace GameTrilha.API.ViewModels.BoardViewModels;

public class ListBoardViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string StrokeColor { get; set; }
    public string BulletColor { get; set; }
    public string ImageSrc { get; set; }
    public double Price { get; set; }

    public ListBoardViewModel(Guid id, string name, string? description, string strokeColor, string bulletColor, string imageSrc, double price)
    {
        Id = id;
        Name = name;
        Description = description;
        StrokeColor = strokeColor;
        BulletColor = bulletColor;
        ImageSrc = imageSrc;
        Price = price;
    }
}