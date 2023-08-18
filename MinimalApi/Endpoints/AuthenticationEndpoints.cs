using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using MinimalApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace MinimalApi.Endpoints;

public static class AuthenticationEndpoints
{

    public static void AddAuthenticationEndpoints(this WebApplication app)
    {
        app.MapPost("/api/token", (IConfiguration config, [FromBody] AuthenticationData data) =>
        {
            var user = ValidateCredentials(data);

            if (user is null)
            {
                return Results.Unauthorized();
            }

            string token = GenerateToken(user, config);

            return Results.Ok(token);
        });
    }

    public static string GenerateToken(UserData user, IConfiguration config)
    {
        var secretKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(
            config.GetValue<string>("Authentication:SecretKey")));

        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims = new();
        claims.Add(new(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
        claims.Add(new(JwtRegisteredClaimNames.UniqueName, user.Username));
        claims.Add(new(JwtRegisteredClaimNames.GivenName, user.FirstName));
        claims.Add(new(JwtRegisteredClaimNames.FamilyName, user.LastName));

        var token = new JwtSecurityToken(
            config.GetValue<string>("Authentication:Issuer"),
            config.GetValue<string>("Authentication:Audience"),
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(1),
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserData? ValidateCredentials(AuthenticationData data)
    {
        // !!!!!
        // This is not production code. Demo use only
        // !!!!!
        if (CompareValues(data.Username, "devtest") &&
            CompareValues(data.Password, "Test1234"))
        {
            return new UserData(1, "Dev", "Test", data.Username!);
        }

        if (CompareValues(data.Username, "client") &&
            CompareValues(data.Password, "Test123"))
        {
            return new UserData(2, "Client", "Test", data.Username!);
        }

        return null;

    }

    private static bool CompareValues(string? actual, string expected)
    {
        if (actual is not null)
        {
            if (actual == expected)
            {
                return true;
            }
        }

        return false;
    }

}
