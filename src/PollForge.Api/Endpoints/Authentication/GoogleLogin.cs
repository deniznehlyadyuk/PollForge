
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PollForge.Api.Extensions;
using PollForge.Api.Infrastructure;
using PollForge.Application.Users.GoogleLogin;
using PollForge.SharedKernel;

namespace PollForge.Api.Endpoints.Authentication;

public class GoogleLogin : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/google-login", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var command = new GoogleLoginCommand(request.Code, request.CodeVerifier);

            Result result = await sender.Send(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        }).WithTags(Tags.Users);
    }

    public sealed record Request(string Code, string CodeVerifier);
}
