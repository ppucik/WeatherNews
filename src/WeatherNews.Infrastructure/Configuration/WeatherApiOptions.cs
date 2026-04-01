using System.ComponentModel.DataAnnotations;

namespace WeatherNews.Infrastructure.Configuration;

public class WeatherApiOptions
{
    public const string SECTION_NAME = "WeatherApi";

    [Required, Url]
    public required string BaseUrl { get; init; }
}
