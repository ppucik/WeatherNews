using WeatherNews.Application.Common;
using WeatherNews.Domain.Entities;
using WeatherNews.Domain.Enums;

namespace WeatherNews.Application.Abstractions;

public interface IWeatherProvider
{
    Task<Result<TemperatureReading>> GetTemperatureAsync(
        CityId cityId,
        CancellationToken cancellationToken = default);
}

