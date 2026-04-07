using WeatherNews.Domain.Enums;

namespace WeatherNews.Domain.Entities;

public sealed record TemperatureReading(
    CityId CityId,
    decimal TemperatureC,
    string Description,
    DateTime MeasuredAtUtc
);
