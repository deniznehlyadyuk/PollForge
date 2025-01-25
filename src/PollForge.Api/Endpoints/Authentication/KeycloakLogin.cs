
using MediatR;
using PollForge.Api.Extensions;
using PollForge.Api.Infrastructure;
using PollForge.Application.Users.KeycloakLogin;

namespace PollForge.Api.Endpoints.Authentication;

public class KeycloakLogin : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new LoginCommand(request.Code, request.CodeVerifier);

            var result = await sender.Send(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Auth);
    }

    private sealed record Request(string Code, string CodeVerifier);
}
