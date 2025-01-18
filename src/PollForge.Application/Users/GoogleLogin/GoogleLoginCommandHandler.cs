using Application.Abstractions.Data;
using MediatR;
using Microsoft.Extensions.Configuration;
using PollForge.Application.Abstractions.Authentication;
using PollForge.Application.Abstractions.Messaging;
using PollForge.Domain.UserEntity;
using PollForge.SharedKernel;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Threading;

namespace PollForge.Application.Users.GoogleLogin;

internal sealed class GoogleLoginCommandHandler(
    IPollForgeDbContext dbContext,
    ITokenProvider tokenProvider,
    IConfiguration configuration) : ICommandHandler<GoogleLoginCommand>
{
    public async Task<Result> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        var response = await SendTokenRequest(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Result.Failure(Error.None);
        }

        var idTokenResult = await ParseIdToken(response, cancellationToken);

        if (idTokenResult.IsFailure)
        {
            return idTokenResult;
        }

        var fullName = idTokenResult.Value.Claims.First(claim => claim.Type == "name")!.Value;
        var email = idTokenResult.Value.Claims.First(claim => claim.Type == "email")!.Value;

        var user = User.Create(fullName, email);
        
        await dbContext.Users.AddAsync(user, cancellationToken);

        var token = tokenProvider.Create(user);

        await dbContext.SaveChangesAsync();

        return Result.Success();
    }

    private async Task<HttpResponseMessage> SendTokenRequest(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        using var client = new HttpClient();

        var url = "https://oauth2.googleapis.com/token";

        var parameters = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("client_id", configuration["GoogleOAuth:ClientId"]!),
            new KeyValuePair<string, string>("client_secret", configuration["GoogleOAuth:ClientSecret"]!),
            new KeyValuePair<string, string>("code", request.Code),
            new KeyValuePair<string, string>("code_verifier", request.CodeVerifier),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("redirect_uri", configuration["GoogleOAuth:RedirectUri"]!)
        ]);

        return await client.PostAsync(url, parameters, cancellationToken);
    }

    private async Task<Result<JwtSecurityToken>> ParseIdToken(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        using var document = JsonDocument.Parse(content);

        if (!document.RootElement.TryGetProperty("id_token", out var idTokenElement))
        {
            return Result.Failure<JwtSecurityToken>(Error.None);
        }

        var idTokenString = idTokenElement.ToString()!;

        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(idTokenString))
        {
            return Result.Failure<JwtSecurityToken>(Error.None);
        }

        return handler.ReadJwtToken(idTokenString);
    }
}