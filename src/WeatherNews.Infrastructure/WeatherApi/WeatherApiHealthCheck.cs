using Microsoft.Extensions.Diagnostics.HealthChecks;
using WeatherNews.Application.Abstractions;
using WeatherNews.Domain.Enums;

namespace WeatherNews.Infrastructure.WeatherApi;

public class WeatherApiHealthCheck(IWeatherProvider weatherProvider) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Skúsime zavolať nejakú metódu "Ping" alebo predpoveď pre jedno mesto
            var result = await weatherProvider.GetTemperatureAsync(CityId.Bratislava, cancellationToken);

            // Kontrola Result objektu
            if (result.IsSuccess)
            {
                return HealthCheckResult.Healthy("Weather API is responding correctly.");
            }

            // Ak Result hlási chybu, vrátime Unhealthy/Degraded aj s popisom chyby
            return HealthCheckResult.Unhealthy($"Weather API returned an error: {result.Error}");
        }
        catch (Exception ex)
        {
            // Toto zachytí len totálne zlyhania (napr. nesprávna URL v konfigurácii)
            return HealthCheckResult.Unhealthy("Weather API check failed with an exception.", ex);
        }
    }
}