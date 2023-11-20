namespace GameTrilha.Domain.Entities;

public class Skin
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Src { get; set; }
    public string? Description { get; set; }

    public double Price { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<User> UsersOwn { get; set; }

    public Skin()
    {
        
    }

    public Skin(string name, string src, string? description, double price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Src = src;
        Description = description;
        Price = price;
    }
}