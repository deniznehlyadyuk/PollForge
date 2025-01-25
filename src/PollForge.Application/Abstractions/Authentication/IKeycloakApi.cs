using PollForge.SharedKernel;

namespace PollForge.Application.Abstractions.Authentication;

public interface IKeycloakApi
{
    Task<Result<KeycloakTokenResponse>> Token(string code, string codeVerifier, CancellationToken cancellationToken);
    Task<Result<KeycloakTokenResponse>> Token(string refreshToken, CancellationToken cancellationToken);
}