using WeatherNews.API.Services;

namespace WeatherNews.API.Endpoints;

public static class GrpcEndpoints
{
    public static IEndpointRouteBuilder MapGrpcEndpoints(this IEndpointRouteBuilder app)
    {
        // 1. Mapovanie samotnej gRPC služby
        app.MapGrpcService<WeatherGrpcService>();

        // 2. Pomocný informačný endpoint (voliteľné)
        app.MapGet("/grpc", () =>
            "Communication is running over gRPC port (8081). " +
            "Use a gRPC client like Postman or grpcui with weather.proto.");

        return app;
    }
}
