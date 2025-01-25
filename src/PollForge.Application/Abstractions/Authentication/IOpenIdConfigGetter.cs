using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace PollForge.Application.Abstractions.Authentication;

public interface IOpenIdConfigGetter
{
    Task<OpenIdConnectConfiguration> Get();
}