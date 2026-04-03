using System.Text.Json.Serialization;

namespace WeatherNews.Infrastructure.WeatherApi;

public record WeatherApiResponse(
    Location Location,
    Current Current
);

public record Location(
    string Name,
    string Region,
    string Country,
    double Lat,
    double Lon,
    [property: JsonPropertyName("tz_id")] string TzId,
    [property: JsonPropertyName("localtime_epoch")] long LocaltimeEpoch,
    string Localtime
);

public record Current(
    [property: JsonPropertyName("last_updated_epoch")] long LastUpdatedEpoch,
    [property: JsonPropertyName("last_updated")] string LastUpdated,
    [property: JsonPropertyName("temp_c")] double TempC,
    [property: JsonPropertyName("temp_f")] double TempF,
    [property: JsonPropertyName("is_day")] int IsDay,
    Condition Condition,
    [property: JsonPropertyName("wind_kph")] double WindKph,
    [property: JsonPropertyName("precip_mm")] double PrecipMm,
    int Humidity,
    int Cloud,
    [property: JsonPropertyName("feelslike_c")] double FeelslikeC,
    double VisKm
);

public record Condition(
    string Text,
    string Icon,
    int Code
);