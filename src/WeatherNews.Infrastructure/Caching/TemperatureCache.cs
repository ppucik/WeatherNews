using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using WeatherNews.Application.Abstractions;
using WeatherNews.Domain.Entities;
using WeatherNews.Domain.Enums;

namespace WeatherNews.Infrastructure.Caching;

public sealed class TemperatureCache(HybridCache cache, ILogger<TemperatureCache> logger, IDateTimeProvider clock)
    : ITemperatureCache
{
    private static string GetKey(CityId cityId) => $"temperature:{(int)cityId}";

    public async Task<TemperatureReading?> GetAsync(CityId cityId, CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync(
            GetKey(cityId),
            async _ => null as TemperatureReading,
            cancellationToken: cancellationToken);
    }

    public async Task SetAsync(TemperatureReading reading, CancellationToken cancellationToken = default)
    {
        var key = GetKey(reading.CityId);
        var now = clock.UtcNow;
        var ttl = TemperatureCacheTime.CalculateTtlUntilNextChange(now);

        await cache.SetAsync(
            key,
            reading,
            new HybridCacheEntryOptions
            {
                Expiration = ttl
            },
            null,
            cancellationToken);

        logger.LogInformation(
            "Cached temperature for {City} until {ExpirationUtc}",
            reading.CityId,
            now + ttl);
    }
}

