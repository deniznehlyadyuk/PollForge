using MediatR;
using PollForge.Api.Extensions;
using PollForge.Api.Infrastructure;
using PollForge.Application.Authentication.Me;

namespace PollForge.Api.Endpoints.Authentication;

public class Me : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("auth/me", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new MeCommand();

            var result = await sender.Send(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        }).WithTags(Tags.Auth).RequireAuthorization();
    }
}