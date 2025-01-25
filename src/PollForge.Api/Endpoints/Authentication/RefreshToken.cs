using MediatR;
using PollForge.Api.Extensions;
using PollForge.Api.Infrastructure;
using PollForge.Application.Users.RefreshToken;

namespace PollForge.Api.Endpoints.Authentication;

public class RefreshToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/refresh-token", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new RefreshTokenCommand();

            var result = await sender.Send(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Auth);
    }
}
