using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using PollForge.Application.Abstractions.Messaging;
using PollForge.SharedKernel;

namespace PollForge.Application.Users.Me;

internal sealed class MeCommandHandler(IHttpContextAccessor httpContextAccessor) : ICommandHandler<MeCommand, MeCommandResponse>
{
    public Task<Result<MeCommandResponse>> Handle(MeCommand request, CancellationToken cancellationToken)
    {
        var email = httpContextAccessor.HttpContext.User.Claims
            .First(claim => claim.Type == JwtRegisteredClaimNames.Email).Value;
        
        var fullName = httpContextAccessor.HttpContext.User.Claims
            .First(claim => claim.Type == JwtRegisteredClaimNames.Name).Value;

        return Task.FromResult(Result.Success(new MeCommandResponse(email, fullName)));
    }
}