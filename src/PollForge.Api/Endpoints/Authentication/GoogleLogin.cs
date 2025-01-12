
namespace PollForge.Api.Endpoints.Authentication;

public class GoogleLogin : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("google-login", () =>
        {
            Console.WriteLine("Hello World!");
        }).RequireAuthorization();
    }
}
