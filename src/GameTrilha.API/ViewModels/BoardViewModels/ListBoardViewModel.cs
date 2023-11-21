namespace GameTrilha.API.ViewModels.BoardViewModels;

public class ListBoardViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string LineColor { get; set; }
    public string BulletColor { get; set; }
    public string BorderLineColor { get; set; }
    public string BackgroundImageSrc { get; set; }
    public double Price { get; set; }
    public bool Selected { get; set; }

    public ListBoardViewModel(Guid id, string name, string? description, string lineColor, string bulletColor, string borderLineColor, string imageSrc, double price, bool selected = false)
    {
        Id = id;
        Name = name;
        Description = description;
        LineColor = lineColor;
        BulletColor = bulletColor;
        BorderLineColor = borderLineColor;
        BackgroundImageSrc = imageSrc;
        Price = price;
        Selected = selected;
    }
}