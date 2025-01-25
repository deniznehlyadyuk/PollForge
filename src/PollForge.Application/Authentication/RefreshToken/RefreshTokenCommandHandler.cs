using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PollForge.Application.Abstractions.Authentication;
using PollForge.Application.Abstractions.Data;
using PollForge.Application.Abstractions.Messaging;
using PollForge.SharedKernel;

namespace PollForge.Application.Authentication.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IPollForgeDbContext dbContext,
    IKeycloakApi keycloakApi,
    IHttpContextAccessor httpContextAccessor,
    IDateTimeProvider dateTimeProvider,
    IJwtValidator jwtValidator) : ICommandHandler<RefreshTokenCommand>
{
    public async Task<Result> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (!httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("ID_TOKEN", out var idToken))
        {
            return Result.Failure(Error.None);
        }

        if (!await jwtValidator.ValidateTokenAsync(idToken))
        {
            return Result.Failure(Error.None);
        }
        
        var handler = new JwtSecurityTokenHandler();
        
        if (!handler.CanReadToken(idToken))
        {
            return Result.Failure(Error.None);
        }
        
        var decodedIdToken = handler.ReadJwtToken(idToken);
        
        var email = decodedIdToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Email).Value;

        var user = await dbContext.Users
            .Include(user => user.Sessions)
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);

        if (user is null)
        {
            return Result.Failure(Error.None);
        }
        
        var sessionState = decodedIdToken.Claims.First(claim => claim.Type == JwtRegisteredClaimNames.Sid).Value;
        
        var session = user.Sessions.FirstOrDefault(session => session.SessionState == sessionState);

        if (session is null)
        {
            return Result.Failure(Error.None);
        }

        var keycloakResponseResult = await keycloakApi.Token(session.RefreshToken, cancellationToken);

        if (keycloakResponseResult.IsFailure)
        {
            return keycloakResponseResult;
        }

        var keycloakResponse = keycloakResponseResult.Value;

        var refreshTokenExp = dateTimeProvider.UtcNow.AddSeconds(keycloakResponse.RefreshExpiresIn);

        user.RefreshSession(session, keycloakResponse.RefreshToken, refreshTokenExp);
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        httpContextAccessor.HttpContext.Response.Cookies.Append("ID_TOKEN", keycloakResponse.IdToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshTokenExp
        });
        
        return Result.Success();
    }
}