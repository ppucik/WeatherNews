using WeatherNews.Domain.Entities;
using WeatherNews.Domain.Enums;

namespace WeatherNews.Application.Abstractions;

public interface ITemperatureCache
{
    Task<TemperatureReading?> GetAsync(CityId cityId, CancellationToken cancellationToken = default);
    Task SetAsync(TemperatureReading reading, CancellationToken cancellationToken = default);
}
