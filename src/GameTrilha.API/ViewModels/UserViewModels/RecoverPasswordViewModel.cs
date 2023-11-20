using System.ComponentModel.DataAnnotations;

namespace GameTrilha.API.ViewModels.UserViewModels;

public class RecoverPasswordViewModel
{
    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    [MaxLength(50)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [MinLength(6)]
    [MaxLength(50)]
    [DataType(DataType.Password)]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }

    [Required]
    [MinLength(6)]
    [MaxLength(6)]
    public string Code { get; set; }
}