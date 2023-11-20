using GameTrilha.Domain.Entities;

namespace GameTrilha.Domain.ValueObjects;

public class UserAvatar
{
    public string Sex { get; set; }
    public string FaceColor { get; set; }
    public string EarSize { get; set; }
    public string HairColor { get; set; }
    public string HairStyle { get; set; }
    public bool? HairColorRandom { get; set; }
    public string HatColor { get; set; }
    public string HatStyle { get; set; }
    public string EyeStyle { get; set; }
    public string GlassesStyle { get; set; }
    public string NoseStyle { get; set; }
    public string MouthStyle { get; set; }
    public string ShirtStyle { get; set; }
    public string ShirtColor { get; set; }
    public string BgColor { get; set; }
    public bool? IsGradient { get; set; }
    public string EyeBrowStyle { get; set; }
}