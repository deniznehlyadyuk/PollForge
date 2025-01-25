using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PollForge.Application.Abstractions.Authentication;
using PollForge.Application.Abstractions.Data;
using PollForge.Application.Abstractions.Messaging;
using PollForge.Domain.UserEntity;
using PollForge.SharedKernel;

namespace PollForge.Application.Authentication.Login;

internal sealed class LoginCommandHandler(
    IPollForgeDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IDateTimeProvider dateTimeProvider,
    IKeycloakApi keycloakApi) : ICommandHandler<LoginCommand>
{
    public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var keycloakResponseResult = await keycloakApi.Token(request.Code, request.CodeVerifier, cancellationToken);

        if (keycloakResponseResult.IsFailure)
        {
            return keycloakResponseResult;
        }

        var keycloakResponse = keycloakResponseResult.Value;
        
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == keycloakResponse.UserInfo.Email, cancellationToken);

        if (user == null)
        {
            user = User.Create(keycloakResponse.UserInfo.FullName, keycloakResponse.UserInfo.Email);
            await dbContext.Users.AddAsync(user, cancellationToken);
        }
        else if (user.FullName != keycloakResponse.UserInfo.FullName)
        {
            user.UpdateFullName(keycloakResponse.UserInfo.FullName);
        }

        var refreshTokenExp = dateTimeProvider.UtcNow.AddSeconds(keycloakResponse.RefreshExpiresIn);
        
        user.AddSession(keycloakResponse.SessionState, keycloakResponse.RefreshToken, dateTimeProvider.UtcNow, refreshTokenExp);
        
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