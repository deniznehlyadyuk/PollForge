using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using PollForge.Application.Abstractions.Authentication;

namespace PollForge.Infrastructure.Authentication;

internal sealed class JwtValidator : IJwtValidator
{
    private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
    private readonly IConfiguration _configuration;
    
    public JwtValidator(IConfiguration configuration)
    {
        var httpClient = new HttpClient();
        
        _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            configuration["Authentication:DiscoveryUrl"],
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever(httpClient)
            {
                RequireHttps = false
            }
        );

        _configuration = configuration;
    }
    
    public async Task<bool> ValidateTokenAsync(string token)
    {
        var jwtHandler = new JwtSecurityTokenHandler();

        var openIdConfig = await _configurationManager.GetConfigurationAsync(CancellationToken.None);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _configuration["Authentication:Issuer"],
            ValidateAudience = true,
            ValidAudience = _configuration["Authentication:Audience"],
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