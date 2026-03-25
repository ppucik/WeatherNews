using WeatherNews.Application.Common;
using WeatherNews.Domain.Entities;
using WeatherNews.Domain.Enums;

namespace WeatherNews.Application.Abstractions;

public interface ITemperatureService
{
    Task<Result<TemperatureReading>> GetCurrentTemperatureAsync(
        CityId cityId,
        CancellationToken cancellationToken = default);
}
