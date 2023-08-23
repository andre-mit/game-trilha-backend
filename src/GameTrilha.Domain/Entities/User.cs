using System.ComponentModel.DataAnnotations;

namespace GameTrilha.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; private set; }

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; private set; }

    [MaxLength]
    public string? Password { get; private set; }

    public double Balance { get; private set; }

    public ICollection<Skin> Skins { get; set; } = new List<Skin>();

    public User()
    {
        
    }

    public User(string name, string email, string password)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Password = password;
        Balance = 0;
    }

    public void AddBalance(double value)
    {
        if(value <= 0)
            throw new ArgumentException("Value must be greater than zero", nameof(value));

        Balance += value;
    }

    public void RemoveBalance(double value)
    {
        if(value <= 0)
            throw new ArgumentException("Value must be greater than zero", nameof(value));
        if(value > Balance)
            throw new ArgumentException("Value must be less than balance", nameof(value));
        Balance -= value;
    }

    public void SetPassword(string password)
    {
        Password = password;
    }

    public void ChangeName(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}