using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherNews.API.Configuration;

namespace Weather.Api.Auth;

public static class JwtAuthExtensions
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services)
    {
        using var serviceProvider = services.BuildServiceProvider();
        var authOptions = serviceProvider.GetRequiredService<IOptions<AuthOptions>>().Value;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authOptions.Issuer,
                    ValidAudience = authOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.JwtKey))
                };
            });

        services.AddAuthorization();

        return services;
    }

    internal static void LogDevToken(this IServiceProvider serviceProvider)
    {
        var authOptions = serviceProvider.GetRequiredService<IOptions<AuthOptions>>().Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.JwtKey));

        var token = new JwtSecurityToken(
            issuer: authOptions.Issuer,
            audience: authOptions.Audience,
            claims: [new Claim(ClaimTypes.Name, authOptions.UserName)],
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        Console.WriteLine($"\n🔑 DEV TOKEN:\n{tokenString}\n");
    }
}
