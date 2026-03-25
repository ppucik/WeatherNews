using Scalar.AspNetCore;

namespace Weather.Api.Docs;

public static class ScalarExtensions
{
    public static IEndpointRouteBuilder MapScalarDocs(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapScalarApiReference(options =>
        {
            options.WithTitle("Weather Temperature API")
                   .WithTheme(ScalarTheme.Default)
                   .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        return endpoints;
    }
}