using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PollForge.Application.Abstractions.Authentication;
using PollForge.SharedKernel;

namespace PollForge.Infrastructure.Authentication;

public class KeycloakApi(IConfiguration configuration, IOpenIdConfigGetter openIdConfigGetter) : IKeycloakApi
{
    public async Task<Result<KeycloakTokenResponse>> Token(string code, string codeVerifier, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();

        var openIdConfig = await openIdConfigGetter.Get();

        var parameters = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("client_id", configuration["Keycloak:ClientId"]!),
            new KeyValuePair<string, string>("client_secret", configuration["Keycloak:ClientSecret"]!),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("code_verifier", codeVerifier),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("redirect_uri", configuration["Authentication:RedirectUri"]!)
        ]);

        var response = await client.PostAsync(openIdConfig.TokenEndpoint, parameters, cancellationToken);

        return await ParseResponse(response, cancellationToken);
    }

    public async Task<Result<KeycloakTokenResponse>> Token(string refreshToken, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();

        var openIdConfig = await openIdConfigGetter.Get();

        var parameters = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("client_id", configuration["Keycloak:ClientId"]!),
            new KeyValuePair<string, string>("client_secret", configuration["Keycloak:ClientSecret"]!),
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken),
        ]);

        var response = await client.PostAsync(openIdConfig.TokenEndpoint, parameters, cancellationToken);

        return await ParseResponse(response, cancellationToken);
    }

    public async Task<Result> Logout(string refreshToken, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();

        var openIdConfig = await openIdConfigGetter.Get();

        var parameters = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("client_id", configuration["Keycloak:ClientId"]!),
            new KeyValuePair<string, string>("client_secret", configuration["Keycloak:ClientSecret"]!),
            new KeyValuePair<string, string>("refresh_token", refreshToken),
        ]);

        var response = await client.PostAsync(openIdConfig.EndSessionEndpoint, parameters, cancellationToken);

        return response.IsSuccessStatusCode ? Result.Success() : Result.Failure(Error.None);
    }

    private static async Task<Result<KeycloakTokenResponse>> ParseResponse(HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        using var document = JsonDocument.Parse(content);

        if (!document.RootElement.TryGetProperty("id_token", out var idTokenElement))
        {
            return Result.Failure<KeycloakTokenResponse>(Error.None);
        }
        
        if (!document.RootElement.TryGetProperty("refresh_token", out var refreshTokenElement))
        {
            return Result.Failure<KeycloakTokenResponse>(Error.None);
        }
        
        if (!document.RootElement.TryGetProperty("refresh_expires_in", out var refreshExpiresInElement))
        {
            return Result.Failure<KeycloakTokenResponse>(Error.None);
        }
        
        if (!document.RootElement.TryGetProperty("session_state", out var sessionStateElement))
        {
            return Result.Failure<KeycloakTokenResponse>(Error.None);
        }
        
        var idToken = idTokenElement.GetString()!;
        var refreshToken = refreshTokenElement.GetString()!;
        var refreshExpiresIn = refreshExpiresInElement.GetInt32();
        var sessionState = sessionStateElement.GetString()!;
        
        var handler = new JwtSecurityTokenHandler();
        
        if (!handler.CanReadToken(idToken) || !handler.CanReadToken(refreshToken))
        {
            return Result.Failure<KeycloakTokenResponse>(Error.None);
        }
        
        var decodedIdToken = handler.ReadJwtToken(idToken);
        
        var fullName = decodedIdToken.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value;
        var email = decodedIdToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;

        if (fullName is null || email is null)
        {
            return Result.Failure<KeycloakTokenResponse>(Error.None);
        }

        return Result.Success(new KeycloakTokenResponse(refreshExpiresIn, refreshToken, idToken, sessionState,
            new UserInfo(fullName, email)));
    }
}