using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Reflection;
using WeatherNews.API.Extensions;

namespace WeatherNews.API.Endpoints;

public static class ServiceEndpoints
{
    public static void MapServiceInfoEndpoints(this IEndpointRouteBuilder app)
    {
        // --- 1. Root endpoint s informáciami o službe ---
        app.MapGet("/", (IHostEnvironment env) =>
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ?? "1.0.0";

            return Results.Ok(new
            {
                Service = "WeatherNews.API",
                Status = "Healthy",
                Version = version,
                Environment = env.EnvironmentName,
                Timestamp = DateTime.UtcNow
            });
        })
        .WithName("GetServiceInfo");

        // ---  2. Health Check Endpoint ---
        app.MapHealthChecks("/health")
            .WithName("GetHealth");

        // Liveness (základná kontrola procesu)
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = HealthCheckFormatter.WriteJsonResponse
        }).WithName("GetLiveness");

        // Readiness (kontrola so všetkými závislosťami)
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = HealthCheckFormatter.WriteJsonResponse
        }).WithName("GetReadiness");

        // --- 3. Základný Metrics Endpoint ---
        app.MapGet("/metrics", () =>
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            return Results.Ok(new
            {
                MemoryUsageBytes = process.WorkingSet64,
                CpuTimeSeconds = process.TotalProcessorTime.TotalSeconds,
                ThreadCount = process.Threads.Count,
                Uptime = DateTime.UtcNow - process.StartTime.ToUniversalTime()
            });
        })
        .WithName("GetMetrics");
    }
}