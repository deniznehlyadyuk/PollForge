using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using PollForge.Application.Abstractions.Authentication;

namespace PollForge.Infrastructure.Authentication;

internal sealed class OpenIdConfigGetter : IOpenIdConfigGetter
{
    private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;

    public OpenIdConfigGetter(IConfiguration configuration)
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
    }
    
    public async Task<OpenIdConnectConfiguration> Get()
    {
        return await _configurationManager.GetConfigurationAsync(CancellationToken.None);
    }
}