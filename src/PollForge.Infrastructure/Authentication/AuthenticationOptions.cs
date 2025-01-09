namespace PollForge.Infrastructure.Authentication;

public sealed class AuthenticationOptions
{
    public string Audience { get; init; } = string.Empty;

    public string MetadataAddress { get; init; } = string.Empty;

    public bool RequireHttpsMetadata { get; init; }

    public string ValidIssuer { get; set; } = string.Empty;
}