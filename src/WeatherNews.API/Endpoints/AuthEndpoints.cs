using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherNews.API.Configuration;

namespace WeatherNews.API.Endpoints;

public record LoginRequest(string Username, string Password);

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", (LoginRequest request, IOptions<AuthOptions> options) =>
        {
            var auth = options.Value;

            if (request.Username == "admin" && request.Password == "admin")
            {
                var token = GenerateJwtToken(auth);
                return Results.Ok(new { Token = token });
            }

            return Results.Unauthorized();
        });

        return app;
    }

    private static string GenerateJwtToken(AuthOptions authOptions)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.JwtKey));

        var token = new JwtSecurityToken(
            issuer: authOptions.Issuer,
            audience: authOptions.Audience,
            claims: [new Claim(ClaimTypes.Name, "admin")],
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
