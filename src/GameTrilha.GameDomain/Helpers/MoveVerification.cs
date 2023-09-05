using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTrilha.GameDomain.Helpers
{
    public static class MoveVerification
    {
        public record Place(byte Track, byte Line, byte Column)
        {
            public override string ToString()
            {
                return $"Track: {Track}, Line: {Line}, Column: {Column}";
            }
        }
        public record Track(Dictionary<Tuple<int, int>, List<Place>> AvailableMoves);

        //public MoveVerification()
        //{
        //var track0 = new Dictionary<Tuple<int, int>, List<Place>>
        //{
        //    [new Tuple<int, int>(0, 0)] = new() { new Place(0, 1,0), new Place(0, 0,1) },
        //    [new Tuple<int, int>(0, 1)] = new() { new Place(0, 0,0), new Place(0, 0,2), new Place(1, 0,1) },
        //    [new Tuple<int, int>(0, 2)] = new() { new Place(0, 1,2), new Place(0, 0,1) },
        //    [new Tuple<int, int>(1, 0)] = new() { new Place(0, 0,0), new Place(1, 1,0), new Place(0, 2,0) },
        //    [new Tuple<int, int>(2, 0)] = new() { new Place(0, 1,0), new Place(0, 2,1) },
        //    [new Tuple<int, int>(2, 1)] = new() { new Place(0, 2,0), new Place(0, 2,2), new Place(1, 2,1) },
        //    [new Tuple<int, int>(2, 2)] = new() { new Place(0, 1,2), new Place(0, 2,1) },
        //    [new Tuple<int, int>(1, 2)] = new() { new Place(0, 0,2), new Place(0, 2,2), new Place(1, 1,2) }
        //};

        //var track1 = new Dictionary<Tuple<int, int>, List<Place>>
        //{
        //    [new Tuple<int, int>(0, 0)] = new() { new Place(1, 1, 0), new Place(1, 0, 1) },
        //    [new Tuple<int, int>(0, 1)] = new() { new Place(1, 0, 0), new Place(1, 0, 2), new Place(0, 0, 1), new Place(2, 0, 1) },
        //    [new Tuple<int, int>(0, 2)] = new() { new Place(1, 1, 2), new Place(1, 0, 1) },
        //    [new Tuple<int, int>(1, 0)] = new() { new Place(1, 0, 0), new Place(1, 2, 0), new Place(0, 1, 0), new Place(2, 1, 0) },
        //    [new Tuple<int, int>(2, 0)] = new() { new Place(1, 1, 0), new Place(1, 2, 1) },
        //    [new Tuple<int, int>(2, 1)] = new() { new Place(1, 2, 0), new Place(1, 2, 2), new Place(0, 2, 1), new Place(2, 2, 1) },
        //    [new Tuple<int, int>(2, 2)] = new() { new Place(1, 1, 2), new Place(1, 2, 1) },
        //    [new Tuple<int, int>(1, 2)] = new() { new Place(1, 0, 2), new Place(1, 2, 2), new Place(0, 1, 2), new Place(2, 1, 2) }
        //};

        //var track2 = new Dictionary<Tuple<int, int>, List<Place>>
        //{
        //    [new Tuple<int, int>(0, 0)] = new() { new Place(2, 1, 0), new Place(2, 0, 1) },
        //    [new Tuple<int, int>(0, 1)] = new() { new Place(2, 0, 0), new Place(2, 0, 2), new Place(1, 0, 1) },
        //    [new Tuple<int, int>(0, 2)] = new() { new Place(2, 1, 2), new Place(2, 0, 1) },
        //    [new Tuple<int, int>(1, 0)] = new() { new Place(2, 0, 0), new Place(2, 2, 0), new Place(1, 1, 0) },
        //    [new Tuple<int, int>(2, 0)] = new() { new Place(2, 1, 0), new Place(2, 2, 1) },
        //    [new Tuple<int, int>(2, 1)] = new() { new Place(2, 2, 0), new Place(2, 2, 2), new Place(1, 2, 1) },
        //    [new Tuple<int, int>(2, 2)] = new() { new Place(2, 1, 2), new Place(2, 2, 1) },
        //    [new Tuple<int, int>(1, 2)] = new() { new Place(2, 0, 2), new Place(2, 2, 2), new Place(1, 1, 2) },
        //};
        //Tracks = new Track[3];
        //Tracks[0] = new Track(track0);
        //Tracks[1] = new Track(track1);
        //Tracks[2] = new Track(track2);
        //}

        public static Track[] Tracks { get; } =
        {
            new(new Dictionary<Tuple<int, int>, List<Place>>
            {
                [new Tuple<int, int>(0, 0)] = new() { new Place(0, 1,0), new Place(0, 0,1) },
                [new Tuple<int, int>(0, 1)] = new() { new Place(0, 0,0), new Place(0, 0,2), new Place(1, 0,1) },
                [new Tuple<int, int>(0, 2)] = new() { new Place(0, 1,2), new Place(0, 0,1) },
                [new Tuple<int, int>(1, 0)] = new() { new Place(0, 0,0), new Place(1, 1,0), new Place(0, 2,0) },
                [new Tuple<int, int>(2, 0)] = new() { new Place(0, 1,0), new Place(0, 2,1) },
                [new Tuple<int, int>(2, 1)] = new() { new Place(0, 2,0), new Place(0, 2,2), new Place(1, 2,1) },
                [new Tuple<int, int>(2, 2)] = new() { new Place(0, 1,2), new Place(0, 2,1) },
                [new Tuple<int, int>(1, 2)] = new() { new Place(0, 0,2), new Place(0, 2,2), new Place(1, 1,2) }
            }),
            new(new Dictionary<Tuple<int, int>, List<Place>>
            {
                [new Tuple<int, int>(0, 0)] = new() { new Place(1, 1, 0), new Place(1, 0, 1) },
                [new Tuple<int, int>(0, 1)] = new() { new Place(1, 0, 0), new Place(1, 0, 2), new Place(0, 0, 1), new Place(2, 0, 1) },
                [new Tuple<int, int>(0, 2)] = new() { new Place(1, 1, 2), new Place(1, 0, 1) },
                [new Tuple<int, int>(1, 0)] = new() { new Place(1, 0, 0), new Place(1, 2, 0), new Place(0, 1, 0), new Place(2, 1, 0) },
                [new Tuple<int, int>(2, 0)] = new() { new Place(1, 1, 0), new Place(1, 2, 1) },
                [new Tuple<int, int>(2, 1)] = new() { new Place(1, 2, 0), new Place(1, 2, 2), new Place(0, 2, 1), new Place(2, 2, 1) },
                [new Tuple<int, int>(2, 2)] = new() { new Place(1, 1, 2), new Place(1, 2, 1) },
                [new Tuple<int, int>(1, 2)] = new() { new Place(1, 0, 2), new Place(1, 2, 2), new Place(0, 1, 2), new Place(2, 1, 2) }
            }),
            new(new Dictionary<Tuple<int, int>, List<Place>>
            {
                [new Tuple<int, int>(0, 0)] = new() { new Place(2, 1, 0), new Place(2, 0, 1) },
                [new Tuple<int, int>(0, 1)] = new() { new Place(2, 0, 0), new Place(2, 0, 2), new Place(1, 0, 1) },
                [new Tuple<int, int>(0, 2)] = new() { new Place(2, 1, 2), new Place(2, 0, 1) },
                [new Tuple<int, int>(1, 0)] = new() { new Place(2, 0, 0), new Place(2, 2, 0), new Place(1, 1, 0) },
                [new Tuple<int, int>(2, 0)] = new() { new Place(2, 1, 0), new Place(2, 2, 1) },
                [new Tuple<int, int>(2, 1)] = new() { new Place(2, 2, 0), new Place(2, 2, 2), new Place(1, 2, 1) },
                [new Tuple<int, int>(2, 2)] = new() { new Place(2, 1, 2), new Place(2, 2, 1) },
                [new Tuple<int, int>(1, 2)] = new() { new Place(2, 0, 2), new Place(2, 2, 2), new Place(1, 1, 2) },
            })
        };

        public static bool MoveAllowed(Place from, Place to)
        {
            return Tracks[from.Track].AvailableMoves
                .TryGetValue(new Tuple<int, int>(from.Line, from.Column), out var places) && places.Contains(to);
        }
    }
}
