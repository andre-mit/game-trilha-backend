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
        public record Track(Dictionary<Tuple<byte, byte>, List<Place>> AvailableMoves);

        public static Track[] Tracks { get; } =
        {
            new(new Dictionary<Tuple<byte, byte>, List<Place>>
            {
                [new Tuple<byte, byte>(0, 0)] = new() { new Place(0, 1,0), new Place(0, 0,1) },
                [new Tuple<byte, byte>(0, 1)] = new() { new Place(0, 0,0), new Place(0, 0,2), new Place(1, 0,1) },
                [new Tuple<byte, byte>(0, 2)] = new() { new Place(0, 1,2), new Place(0, 0,1) },
                [new Tuple<byte, byte>(1, 0)] = new() { new Place(0, 0,0), new Place(1, 1,0), new Place(0, 2,0) },
                [new Tuple<byte, byte>(2, 0)] = new() { new Place(0, 1,0), new Place(0, 2,1) },
                [new Tuple<byte, byte>(2, 1)] = new() { new Place(0, 2,0), new Place(0, 2,2), new Place(1, 2,1) },
                [new Tuple<byte, byte>(2, 2)] = new() { new Place(0, 1,2), new Place(0, 2,1) },
                [new Tuple<byte, byte>(1, 2)] = new() { new Place(0, 0,2), new Place(0, 2,2), new Place(1, 1,2) }
            }),
            new(new Dictionary<Tuple<byte, byte>, List<Place>>
            {
                [new Tuple<byte, byte>(0, 0)] = new() { new Place(1, 1, 0), new Place(1, 0, 1) },
                [new Tuple<byte, byte>(0, 1)] = new() { new Place(1, 0, 0), new Place(1, 0, 2), new Place(0, 0, 1), new Place(2, 0, 1) },
                [new Tuple<byte, byte>(0, 2)] = new() { new Place(1, 1, 2), new Place(1, 0, 1) },
                [new Tuple<byte, byte>(1, 0)] = new() { new Place(1, 0, 0), new Place(1, 2, 0), new Place(0, 1, 0), new Place(2, 1, 0) },
                [new Tuple<byte, byte>(2, 0)] = new() { new Place(1, 1, 0), new Place(1, 2, 1) },
                [new Tuple<byte, byte>(2, 1)] = new() { new Place(1, 2, 0), new Place(1, 2, 2), new Place(0, 2, 1), new Place(2, 2, 1) },
                [new Tuple<byte, byte>(2, 2)] = new() { new Place(1, 1, 2), new Place(1, 2, 1) },
                [new Tuple<byte, byte>(1, 2)] = new() { new Place(1, 0, 2), new Place(1, 2, 2), new Place(0, 1, 2), new Place(2, 1, 2) }
            }),
            new(new Dictionary<Tuple<byte, byte>, List<Place>>
            {
                [new Tuple<byte, byte>(0, 0)] = new() { new Place(2, 1, 0), new Place(2, 0, 1) },
                [new Tuple<byte, byte>(0, 1)] = new() { new Place(2, 0, 0), new Place(2, 0, 2), new Place(1, 0, 1) },
                [new Tuple<byte, byte>(0, 2)] = new() { new Place(2, 1, 2), new Place(2, 0, 1) },
                [new Tuple<byte, byte>(1, 0)] = new() { new Place(2, 0, 0), new Place(2, 2, 0), new Place(1, 1, 0) },
                [new Tuple<byte, byte>(2, 0)] = new() { new Place(2, 1, 0), new Place(2, 2, 1) },
                [new Tuple<byte, byte>(2, 1)] = new() { new Place(2, 2, 0), new Place(2, 2, 2), new Place(1, 2, 1) },
                [new Tuple<byte, byte>(2, 2)] = new() { new Place(2, 1, 2), new Place(2, 2, 1) },
                [new Tuple<byte, byte>(1, 2)] = new() { new Place(2, 0, 2), new Place(2, 2, 2), new Place(1, 1, 2) },
            })
        };

        public static bool MoveAllowed(Place from, Place to)
        {
            return AllowedPlaces(from).Contains(to);
        }

        public static  List<Place> AllowedPlaces(Place where)
        {
            Tracks[where.Track].AvailableMoves
                .TryGetValue(new Tuple<byte, byte>(where.Line, where.Column), out var places);

            if(places == null)
                throw new Exception("Place not found");

            return places;
        }
    }
}
