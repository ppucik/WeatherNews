using System.ComponentModel.DataAnnotations;

namespace WeatherNews.API.Configuration;

public class AuthOptions
{
    public const string SECTION_NAME = "Auth";

    /// <summary>
    /// JWT kľúč by mal mať aspoň 256 bitov (32 znakov)
    /// </summary>
    [Required, MinLength(32)]
    public required string JwtKey { get; init; }

    [Required]
    public required string Issuer { get; init; }

    [Required]
    public required string Audience { get; init; }

    public string UserName { get; init; } = "dev-user";
}
