using System.Text.Json.Serialization;
using GameTrilha.API.Helpers.JsonConverters;
using GameTrilha.GameDomain.Entities;

namespace GameTrilha.API.ViewModels.GameViewModels;

public class TrackViewModel
{
    [JsonConverter(typeof(MultidimensionalObjectArrayJsonConverter<PlaceViewModel>))]
    public PlaceViewModel[,] Places { get; set; }

    public TrackViewModel(PlaceViewModel[,] places)
    {
        Places = places;
    }

    public static implicit operator TrackViewModel(Track track)
    {
        var places = new PlaceViewModel[3, 3];
        for (var i = 0; i < 3; i++)
        for (var j = 0; j < 3; j++)
        {
                if(i==1 && j == 1) continue;

                places[i, j] = track.Places[i, j];
        }

        return new TrackViewModel(places);
    }

    public static TrackViewModel[] ParseTrackArray(Track[] tracks)
    {
        return tracks.Select(track => (TrackViewModel)track).ToArray();
    }
}