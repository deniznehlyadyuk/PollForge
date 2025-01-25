using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PollForge.Application.Abstractions.Messaging;
using PollForge.SharedKernel;

namespace PollForge.Application.Authentication.Me;

internal sealed class MeCommandHandler(IHttpContextAccessor httpContextAccessor) : ICommandHandler<MeCommand, MeCommandResponse>
{
    public Task<Result<MeCommandResponse>> Handle(MeCommand request, CancellationToken cancellationToken)
    {
        var email = httpContextAccessor.HttpContext.User.Claims
            .First(claim => claim.Type == ClaimTypes.Email).Value;
        
        var fullName = httpContextAccessor.HttpContext.User.Claims
            .First(claim => claim.Type == JwtRegisteredClaimNames.Name).Value;

        return Task.FromResult(Result.Success(new MeCommandResponse(email, fullName)));
    }
}