using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using PollForge.Application.Abstractions.Authentication;

namespace PollForge.Infrastructure.Authentication;

internal sealed class JwtValidator(IOpenIdConfigGetter openIdConfigGetter, IConfiguration configuration) : IJwtValidator
{
    public async Task<bool> ValidateTokenAsync(string token)
    {
        var jwtHandler = new JwtSecurityTokenHandler();

        var openIdConfig = await openIdConfigGetter.Get();
        
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = configuration["Authentication:Issuer"],
            ValidateAudience = true,
            ValidAudience = configuration["Authentication:Audience"],
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = openIdConfig.SigningKeys,
        };

        try
        {
            jwtHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return true;
        }
        catch (SecurityTokenException)
        {
            return false;
        }
    }
}