using System.ComponentModel.DataAnnotations;

namespace GameTrilha.API.ViewModels.BoardViewModels;

public class CreateBoardViewModel
{
    [MaxLength(50)]
    public string Name { get; set; }
    [MaxLength]
    public string? Description { get; set; }
    [MaxLength(7)]
    public string LineColor { get; set; }

    [MaxLength(7)]
    public string BorderLineColor { get; set; }

    [MaxLength(7)]
    public string BulletColor { get; set; }
    [MaxLength]
    public string Image { get; set; }
    public string ImageFileName { get; set; }
    [Required]
    public double Price { get; set; }
}