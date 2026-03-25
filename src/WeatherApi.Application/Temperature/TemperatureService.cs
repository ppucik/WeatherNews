using Microsoft.Extensions.Logging;
using WeatherNews.Application.Abstractions;
using WeatherNews.Application.Common;
using WeatherNews.Domain.Entities;
using WeatherNews.Domain.Enums;

namespace WeatherNews.Application.Temperature;

public sealed class TemperatureService : ITemperatureService
{
    private readonly IWeatherProvider _weatherProvider;
    private readonly ITemperatureCache _cache;
    private readonly IDateTimeProvider _clock;
    private readonly ILogger<TemperatureService> _logger;

    public TemperatureService(
        IWeatherProvider weatherProvider,
        ITemperatureCache cache,
        IDateTimeProvider clock,
        ILogger<TemperatureService> logger)
    {
        _weatherProvider = weatherProvider;
        _cache = cache;
        _clock = clock;
        _logger = logger;
    }

    public async Task<Result<TemperatureReading>> GetCurrentTemperatureAsync(
        CityId cityId,
        CancellationToken cancellationToken = default)
    {
        var cached = await _cache.GetAsync(cityId, cancellationToken);
        if (cached is not null)
            return Result<TemperatureReading>.Success(cached);

        var apiResult = await _weatherProvider.GetTemperatureAsync(cityId, cancellationToken);

        if (apiResult.IsSuccess)
        {
            await _cache.SetAsync(apiResult.Value!, cancellationToken);
            return apiResult;
        }

        return Result<TemperatureReading>.Failure("Temperature unavailable.");
    }
}
