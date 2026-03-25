using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherNews.Application.Abstractions;
using WeatherNews.Application.Temperature;
using WeatherNews.Infrastructure.Caching;
using WeatherNews.Infrastructure.Time;
using WeatherNews.Infrastructure.WeatherApi;

namespace WeatherNews.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddHybridCache();

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<ITemperatureCache, TemperatureCache>();

        services.AddHttpClient<IWeatherProvider, WeatherApiClient>(client =>
        {
            client.BaseAddress = new Uri(config["WeatherApi:BaseUrl"]!);
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

        services.AddScoped<ITemperatureService, TemperatureService>();

        return services;
    }
}

