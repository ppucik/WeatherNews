using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WeatherNews.Application.Abstractions;
using WeatherNews.Application.Temperature;
using WeatherNews.Infrastructure.Caching;
using WeatherNews.Infrastructure.Configuration;
using WeatherNews.Infrastructure.Time;
using WeatherNews.Infrastructure.WeatherApi;

namespace WeatherNews.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // 1. Konfigurácia Options pre WeatherApi
        services.AddOptions<WeatherApiOptions>()
            .BindConfiguration(WeatherApiOptions.SECTION_NAME)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // 2. Hybrid cache
        services.AddHybridCache();

        // 3. Singleton služby
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<ITemperatureCache, TemperatureCache>();

        // 4. HttpClient s typovaným klientom a Resilience
        services.AddHttpClient<IWeatherProvider, WeatherApiClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<WeatherApiOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
        })
        .AddStandardResilienceHandler(options =>
        {
            // Konfigurácia Retry (opakovania)
            options.Retry.MaxRetryAttempts = 3;
            options.Retry.Delay = TimeSpan.FromMilliseconds(200);
            options.Retry.BackoffType = Polly.DelayBackoffType.Constant;

            // Pokud jeden pokus trvá dlh3ie ako 1s, zkusí sa další
            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(1);

            // Konfigurácia celkového Timeoutu
            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(5);

            // Konfigurácia Circuit Breakeru
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
            options.CircuitBreaker.FailureRatio = 0.5;
            options.CircuitBreaker.MinimumThroughput = 10;
            options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
        });

        // 5. Scoped služby
        services.AddScoped<ITemperatureService, TemperatureService>();

        return services;
    }
}