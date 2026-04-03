using WeatherNews.Domain.Enums;

namespace WeatherNews.Domain.Entities;

/// <summary>
/// Represents a temperature reading for a specific city at a given point in time.
/// </summary>
/// <param name="CityId">The city for which the temperature was measured.</param>
/// <param name="TemperatureC">The temperature value, in degrees Celsius, recorded for the city.</param>
/// <param name="Description"></param>
/// <param name="MeasuredAtUtc">The date and time, in Coordinated Universal Time (UTC), when the temperature was measured.</param>
public sealed record TemperatureReading(
    CityId CityId,
    decimal TemperatureC,
    string Description,
    DateTime MeasuredAtUtc
);
