using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PollForge.Application.Abstractions.Authentication;
using PollForge.Application.Abstractions.Data;
using PollForge.Application.Abstractions.Messaging;
using PollForge.SharedKernel;

namespace PollForge.Application.Authentication.Logout;

internal sealed class LogoutCommandHandler(
    IPollForgeDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IKeycloakApi keycloakApi) : ICommandHandler<LogoutCommand>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (!httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("ID_TOKEN", out var idToken))
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
        
        var logoutResult = await keycloakApi.Logout(session.RefreshToken, cancellationToken);

        if (logoutResult.IsFailure)
        {
            return logoutResult;
        }
        
        httpContextAccessor.HttpContext.Response.Cookies.Delete("ID_TOKEN");
        
        return Result.Success();
    }
}