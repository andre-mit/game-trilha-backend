using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GameTrilha.Domain.Entities;

public class RecoveryPasswordCode
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(6)]
    public string Code { get; set; }

    [Required]
    public DateTime ExpiresAt { get; set; }

    [Required]
    [DefaultValue(false)]
    public bool Locked { get; set; }

    [Required]
    public Guid UserId { get; set; }
    public User User { get; set; }

    public RecoveryPasswordCode()
    {
        
    }

    public RecoveryPasswordCode(string code, DateTime expiresAt, bool locked, Guid userId)
    {
        Id = Guid.NewGuid();
        Code = code;
        ExpiresAt = expiresAt;
        Locked = locked;
        UserId = userId;
    }
}