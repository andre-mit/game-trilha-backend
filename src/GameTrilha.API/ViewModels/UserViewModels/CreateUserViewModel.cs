using System.ComponentModel.DataAnnotations;

namespace GameTrilha.API.ViewModels.UserViewModels;

public class CreateUserViewModel
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

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
}