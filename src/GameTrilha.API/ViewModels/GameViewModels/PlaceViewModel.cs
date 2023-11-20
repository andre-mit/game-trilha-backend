using GameTrilha.GameDomain.Entities;
using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.ViewModels.GameViewModels;

public class PlaceViewModel
{
    public Color? Color { get; set; }

    public PlaceViewModel(Color? color)
    {
        Color = color;
    }

    public static implicit operator PlaceViewModel(Place place)
    {
        return new PlaceViewModel(place.Piece?.Color);
    }
}