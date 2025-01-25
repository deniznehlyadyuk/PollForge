namespace PollForge.SharedKernel;

public sealed record UserInfo(string FullName, string Email);

public sealed record KeycloakTokenResponse(
    int RefreshExpiresIn,
    string RefreshToken,    
    string IdToken,
    string SessionState,
    UserInfo UserInfo);