namespace GameTrilha.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public ICollection<UserRole> Users { get; set; }
}