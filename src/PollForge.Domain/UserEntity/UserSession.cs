using PollForge.SharedKernel;

namespace PollForge.Domain.UserEntity;

public class UserSession : ValueObject
{
    public string SessionState { get; private set; } = null!;
    public string RefreshToken { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    
    private UserSession()
    {
    }

    public static UserSession Create(string sessionState, string refreshToken, DateTime createdAtUtc, DateTime expiresAtUtc)
    {
        return new UserSession
        {
            SessionState = sessionState,
            RefreshToken = refreshToken,
            CreatedAtUtc = createdAtUtc,
            ExpiresAtUtc = expiresAtUtc
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SessionState;
        yield return RefreshToken;
        yield return CreatedAtUtc;
        yield return ExpiresAtUtc;
    }
}