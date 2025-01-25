
namespace PollForge.Api.Endpoints.Authentication;

public class AuthorizedEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/authorized-endpoint", () =>
        {
            return Results.Ok();
        }).RequireAuthorization();
    }
}
