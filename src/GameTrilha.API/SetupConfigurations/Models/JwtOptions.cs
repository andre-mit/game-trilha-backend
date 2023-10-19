namespace GameTrilha.API.SetupConfigurations.Models;

public class JwtOptions
{
    /// <summary>
    /// Secret key to generate token
    /// </summary>
    public string Key { get; set; } = "";

    /// <summary>
    /// Who created this token
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// Who can use this token
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// Days to expire token
    /// </summary>
    public int? Expires { get; set; }
}