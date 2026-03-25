using Microsoft.OpenApi;

namespace WeatherNews.API.Docs;

public static class OpenApiExtensions
{
    public static IServiceCollection AddWeatherOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((doc, ctx, ct) =>
            {
                doc.Info.Title = "WeatherApi";
                doc.Info.Version = "v1";
                doc.Info.Description = "Returns current temperature for supported cities. Data refreshes at 09:00 and 16:00 UTC.";

                doc.Components ??= new OpenApiComponents();
                doc.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                doc.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header. Example: 'Bearer {token}'"
                };

                return Task.CompletedTask;
            });
        });


        return services;
    }
}
