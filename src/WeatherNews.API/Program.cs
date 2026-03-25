using Microsoft.AspNetCore.HttpLogging;
using Weather.Api.Auth;
using Weather.Api.Docs;
using WeatherNews.API.Docs;
using WeatherNews.API.Endpoints;
using WeatherNews.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddHttpLogging(o =>
{
    o.LoggingFields = HttpLoggingFields.RequestPath |
                      HttpLoggingFields.RequestMethod |
                      HttpLoggingFields.ResponseStatusCode;
});

builder.Services.AddJwtAuth(config);
builder.Services.AddInfrastructure(config);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddWeatherOpenApi();

var app = builder.Build();

app.UseHttpLogging();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarDocs();
    JwtAuthExtensions.GenerateJwtToken("dev-user", config);
}

app.UseHttpsRedirection();

app.MapTemperatureEndpoints();

app.Run();
