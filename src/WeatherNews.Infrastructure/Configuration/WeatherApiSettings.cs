using System.ComponentModel.DataAnnotations;

namespace WeatherNews.Infrastructure.Configuration;

/// <summary>
/// Represents configuration options for connecting to a weather API service.
/// </summary>
public class WeatherApiOptions
{
    public const string SECTION_NAME = "WeatherApi";

    /// <summary>
    /// Gets the base URL used for constructing API requests.
    /// </summary>
    [Required, Url]
    public required string BaseUrl { get; init; }

    /// <summary>
    /// Gets the API key used to authenticate requests.
    /// </summary>
    public required string ApiKey { get; init; }
}
