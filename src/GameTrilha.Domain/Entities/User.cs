using System.ComponentModel.DataAnnotations;

namespace GameTrilha.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    [MaxLength]
    public string? Password { get; set; }

    public int Balance { get; set; }

    public int Score { get; set; }

    public ICollection<Board> Boards { get; set; }
    public ICollection<Skin> Skins { get; set; } = new List<Skin>();
    public ICollection<Match> MatchesPlayer1 { get; set; }
    public ICollection<Match> MatchesPlayer2 { get; set; }
    public ICollection<Match> Wins { get; set; }

    public ICollection<UserRole> Roles { get; set; }

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

    public void AddBalance(int value)
    {
        if(value <= 0)
            throw new ArgumentException("Value must be greater than zero", nameof(value));

        Balance += value;
    }

    public void RemoveBalance(int value)
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