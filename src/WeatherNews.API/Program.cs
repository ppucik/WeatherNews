using Microsoft.AspNetCore.HttpLogging;
using Weather.Api.Auth;
using Weather.Api.Docs;
using WeatherNews.API.Configuration;
using WeatherNews.API.Docs;
using WeatherNews.API.Endpoints;
using WeatherNews.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Registrácia služieb  ---

// Konfigurácia Options s validáciou
builder.Services.AddOptions<AuthOptions>()
    .BindConfiguration(AuthOptions.SECTION_NAME)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Logovanie HTTP požiadaviek
builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.RequestPath |
                      HttpLoggingFields.RequestMethod |
                      HttpLoggingFields.ResponseStatusCode;
});

// Registrácia vlastných modulov
builder.Services.AddJwtAuth();
builder.Services.AddInfrastructure();

// Dokumentácia
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddWeatherOpenApi();

var app = builder.Build();

// --- 2. Middleware Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarDocs();
    app.Services.LogDevToken();
}

app.UseHttpsRedirection();
app.UseHttpLogging();

app.UseAuthentication();
app.UseAuthorization();

// --- 3. Endpointy ---

app.MapServiceInfoEndpoints();
app.MapTemperatureEndpoints();

app.Run();
