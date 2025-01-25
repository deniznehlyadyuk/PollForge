using MediatR;
using PollForge.Api.Extensions;
using PollForge.Api.Infrastructure;
using PollForge.Application.Authentication.Logout;

namespace PollForge.Api.Endpoints.Authentication;

public class Logout : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/logout", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new LogoutCommand();

            var result = await sender.Send(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Auth).RequireAuthorization();
    }
}