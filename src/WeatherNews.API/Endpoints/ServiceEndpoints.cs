using System.Reflection;

namespace WeatherNews.API.Endpoints;

public static class ServiceEndpoints
{
    public static void MapServiceInfoEndpoints(this IEndpointRouteBuilder app)
    {
        // Root endpoint s informáciami o službe
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

        // 1. Health Check Endpoint
        app.MapHealthChecks("/health")
            .WithName("GetHealth");

        // 2. Základný Metrics Endpoint
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