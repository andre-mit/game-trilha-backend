using GameTrilha.GameDomain.Enums;

namespace GameTrilha.API.Helpers
{
    public static class RandomColor
    {
        public static Color GetRandomColor()
        {
            var random = new Random();
            var colors = Enum.GetValues(typeof(Color));
            return (Color)(colors.GetValue(random.Next(colors.Length)) ?? Color.White);
        }

        public static Color GetOppositeColor(Color color)
        {
            return color == Color.White ? Color.Black : Color.White;
        }
    }
}
