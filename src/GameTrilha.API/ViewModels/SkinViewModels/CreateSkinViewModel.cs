using System.ComponentModel.DataAnnotations;

namespace GameTrilha.API.ViewModels.SkinViewModels;

public class CreateSkinViewModel
{
    [MaxLength(50)]
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Image { get; set; }
    public string ImageFileName { get; set; }
    [Required]
    public double Price { get; set; }
}