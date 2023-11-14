namespace GameTrilha.Domain.DTOs.Skins;

public class SkinDetails
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Src { get; set; }
    public string? Description { get; set; }

    public double Price { get; set; }

    public bool Purchased { get; set; }

    public SkinDetails(Guid id, string name, string src, string? description, double price, bool purchased)
    {
        Id = id;
        Name = name;
        Src = src;
        Description = description;
        Price = price;
        Purchased = purchased;
    }
}