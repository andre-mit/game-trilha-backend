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
    [DataType(DataType.Password)]
    public string Password { get; set; }
}